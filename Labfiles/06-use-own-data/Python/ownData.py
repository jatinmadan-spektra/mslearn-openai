import os
import glob
import dotenv
from openai import OpenAI

dotenv.load_dotenv()

endpoint = os.environ.get("AZURE_OAI_ENDPOINT")
api_key = os.environ.get("AZURE_OAI_KEY")
deployment = os.environ.get("AZURE_OAI_DEPLOYMENT")
vector_store_id = os.environ.get("VECTOR_STORE_ID")

# Initialize client using the Azure OpenAI v1 endpoint
client = OpenAI(
    api_key=api_key,
    base_url=endpoint.rstrip("/") + "/openai/v1/"
)

# Create a vector store from the brochure PDFs (first run only)
if not vector_store_id:
    print("Uploading brochures and creating vector store...")
    file_ids = []
    for path in glob.glob("../data/*.pdf"):
        with open(path, "rb") as f:
            uploaded = client.files.create(file=f, purpose="assistants")
        file_ids.append(uploaded.id)
        print("  Uploaded", os.path.basename(path))

    vector_store = client.vector_stores.create(name="margies-travel-brochures")
    client.vector_stores.file_batches.create_and_poll(
        vector_store_id=vector_store.id,
        file_ids=file_ids
    )

    vector_store_id = vector_store.id
    print("\nVector store ID:", vector_store_id)
    print("Add this value as VECTOR_STORE_ID in .env to reuse it on the next run.\n")

# Get user input
text = input("Enter a question:\n")

# Ask the model, grounded in the brochures with the file search tool
response = client.responses.create(
    model=deployment,
    instructions=(
        "You are a travel assistant for Margie's Travel. Always search the brochures "
        "with the file search tool and answer only from them. If the brochures don't "
        "cover the question, say so."
    ),
    input=text,
    tools=[{"type": "file_search", "vector_store_ids": [vector_store_id]}]
)

# Print response and the source files it cited
print("\nResponse:\n" + response.output_text + "\n")

sources = []
for item in response.output:
    if item.type == "message":
        for part in item.content:
            for annotation in getattr(part, "annotations", []) or []:
                if annotation.type == "file_citation" and annotation.filename not in sources:
                    sources.append(annotation.filename)
                    print("Source:", annotation.filename)
