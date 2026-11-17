using System.ClientModel;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Files;
using OpenAI.Responses;
using OpenAI.VectorStores;

#pragma warning disable OPENAI001

// Get configuration settings from appsettings.json in the project folder
IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

string oaiEndpoint = config["AzureOAIEndpoint"] ?? "";
string oaiKey = config["AzureOAIKey"] ?? "";
string oaiDeploymentName = config["AzureOAIDeploymentName"] ?? "";
string vectorStoreId = config["VectorStoreId"] ?? "";

// Initialize client using the Azure OpenAI v1 endpoint
OpenAIClient client = new(
    new ApiKeyCredential(oaiKey),
    new OpenAIClientOptions { Endpoint = new Uri(oaiEndpoint.TrimEnd('/') + "/openai/v1/") });

OpenAIFileClient fileClient = client.GetOpenAIFileClient();
VectorStoreClient vectorStoreClient = client.GetVectorStoreClient();
ResponsesClient responsesClient = client.GetResponsesClient();

// Create a vector store from the brochure PDFs (first run only)
if (string.IsNullOrEmpty(vectorStoreId))
{
    Console.WriteLine("Uploading brochures and creating vector store...");
    List<string> fileIds = new();
    foreach (string path in Directory.GetFiles("../data", "*.pdf"))
    {
        OpenAIFile file = fileClient.UploadFile(path, FileUploadPurpose.Assistants).Value;
        fileIds.Add(file.Id);
        Console.WriteLine($"  Uploaded {Path.GetFileName(path)}");
    }

    VectorStore vectorStore = vectorStoreClient.CreateVectorStore(
        new VectorStoreCreationOptions { Name = "margies-travel-brochures" }).Value;
    VectorStoreFileBatch batch = vectorStoreClient.AddFileBatchToVectorStore(vectorStore.Id, fileIds).Value;

    while (batch.Status == VectorStoreFileBatchStatus.InProgress)
    {
        Thread.Sleep(2000);
        batch = vectorStoreClient.GetVectorStoreFileBatch(vectorStore.Id, batch.BatchId).Value;
    }

    vectorStoreId = vectorStore.Id;
    Console.WriteLine($"\nVector store ID: {vectorStoreId}");
    Console.WriteLine("Add this value as VectorStoreId in appsettings.json to reuse it on the next run.\n");
}

// Get input
Console.WriteLine("Enter a question:");
string text = Console.ReadLine() ?? "";

// Ask the model, grounded in the brochures with the file search tool
CreateResponseOptions options = new()
{
    Model = oaiDeploymentName,
    Instructions = "You are a travel assistant for Margie's Travel. Always search the brochures with the file search tool and answer only from them. If the brochures don't cover the question, say so.",
    InputItems = { ResponseItem.CreateUserMessageItem(text) },
    Tools = { ResponseTool.CreateFileSearchTool(new[] { vectorStoreId }) }
};

ResponseResult response = responsesClient.CreateResponse(options).Value;

// Print response and the source files it cited
Console.WriteLine($"\nResponse:\n{response.GetOutputText()}\n");

HashSet<string> sources = new();
foreach (MessageResponseItem message in response.OutputItems.OfType<MessageResponseItem>())
{
    foreach (ResponseContentPart part in message.Content)
    {
        foreach (FileCitationMessageAnnotation citation in part.OutputTextAnnotations.OfType<FileCitationMessageAnnotation>())
        {
            if (sources.Add(citation.Filename))
            {
                Console.WriteLine($"Source: {citation.Filename}");
            }
        }
    }
}
