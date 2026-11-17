## Lab 06: Add your data for RAG with Azure OpenAI Service

## Lab scenario
The Azure OpenAI Service enables you to use your own data with the intelligence of the underlying LLM. You can limit the model to only use your data for pertinent topics, or blend it with results from the pre-trained model.

## Lab objectives

In this lab, you will complete the following tasks:

- Task 1: Provision a Microsoft Foundry resource
- Task 2: Deploy a model
- Task 3: Observe normal chat behavior without adding your own data
- Task 4: Create an AI Assistant with your own data
- Task 5: Chat with a model grounded in your data
- Task 6: Set up an application in Cloud Shell
- Task 7: Configure your application
- Task 8: Run your application

## Estimated time: 60 minutes

### Task 1: Provision a Microsoft Foundry  resource

In this task , you'll create an Azure resource in the Azure portal, selecting the Microsoft Foundry service and configuring settings such as region and pricing tier. This setup allows you to integrate OpenAI's advanced language models into your applications.

1. In the **Azure portal**, search for **Microsoft Foundry (1)** and select **Microsoft Foundry (2)** from the results.

   ![](../media/mf-1.png)

1. On the **Microsoft Foundry** overview pane, select **Create a resource**

   ![](../media/mf-2.png)

1. Create an **Foundry** resource using the settings below, then click **Review + create (6)** , leaving all other options at their defaults.
    
    - Subscription: **Default Subscription (1)**
    
    - Resource group: **openai-<inject key="DeploymentID" enableCopy="false"></inject> (2)**
    
    - Name: **OpenAI-Lab06-<inject key="DeploymentID" enableCopy="false"></inject> (3)**

    - Region: **<inject key="Region" enableCopy="false"></inject> (4)**
    
    - Default project name: **proj-default (5)**
  
      ![](../media/mf-3.png)

1. Under the **Review + create** tab, click on **Create**.

1. Wait for deployment to complete. Click on **Go to resource** to navigate to the deployed foundry resource in the Azure portal.

      ![](../media/mf-4.png)

1. To capture the Keys and Endpoints values, on **OpenAI-Lab06-<inject key="DeploymentID" enableCopy="false"></inject>** blade:

    - On the left navigation menu, expand **Resource Management** and select **Keys and Endpoint (1)**.
    
    - Copy **Key 1 (2)** and ensure to paste it in a text editor such as notepad for future reference.
    
    - Select **OpenAI (3)**, copy the **Endpoint (4)** API URL by clicking on copy to clipboard. Paste it in a text editor such as notepad for later use.
    
        ![](../media/mf-5.png "Keys and Endpoints")

<validation step="324aa14e-e7b9-4856-8b46-9a8ce2422139" />

> **Congratulations** on completing the task! Now, it's time to validate it. Here are the steps:
> - Hit the Validate button for the corresponding task. If you receive a success message, you can proceed to the next task. 
> - If not, carefully read the error message and retry the step, following the instructions in the lab guide.
> - If you need any assistance, please contact us at cloudlabs-support@spektrasystems.com. We are available 24/7 to help you out.

### Task 2: Deploy a model

In this task, you'll deploy a specific AI model instance within your Azure OpenAI resource to integrate advanced language capabilities into your applications.

1. In the Foundry resource pane, click on **Go to Foundry portal**, which will navigate to **Microsoft Foundry**.

    ![](../media/mf-6.png)

1. Click **View deployments** under Use a model.

    ![](../media/SS-1.png)
   
1. On Deployments tab, click **Deploy (1)**, and choose **Deploy a base model (2)**.

     ![](../media/SS-2.png)

1. Search for **gpt-5-mini (1)** in the search bar, select **gpt-5-mini (2)**.

     ![](../media/L1T2S3a-0707.png) 

1. On **gpt-5-mini** details page, click on **Custom deploy**.

   ![](../media/L1T2S4-0707.png)

      >**Note:** If the **Custom Deploy** option is not visible, click **Deploy**, then select **Custom settings**.

      >![](../media/mf-7.png)

1. Within the **Deploy gpt-5-mini** pop-up interface, enter the following details:

    - **Deployment name**: **text-turbo (1)**

    - **Deployment type**: **Global Standard (2)**

    - **Tokens per Minute Rate Limit**: **15K (3)**

    - **Guardrails**: **DefaultV2 (4)**

    - Click on **Deploy (5)**

         ![](../media/new-b1.png)

1. This will deploy a model that you will use as you proceed.

<validation step="0fbfe47d-b9b0-49f1-bf2d-adfbb8a8782b" />

> **Congratulations** on completing the task! Now, it's time to validate it. Here are the steps:
> - Hit the Validate button for the corresponding task. If you receive a success message, you can proceed to the next task. 
> - If not, carefully read the error message and retry the step, following the instructions in the lab guide.
> - If you need any assistance, please contact us at cloudlabs-support@spektrasystems.com. We are available 24/7 to help you out.

### Task 3: Observe normal chat behavior without adding your own data

Before connecting Azure OpenAI to your data, first observe how the base model responds to queries without any grounding data.

1. In the Foundry portal, on the **Models** page, select the **text-turbo (1)** deployment and then select **Open in playground (2)**.

1. In the chat pane on the right side of the playground, submit the following queries, and review the responses:

    ```
    I'd like to take a trip to New York. Where should I stay?
    ```

    ```
    What are some facts about New York?
    ```

    Try similar questions about tourism and places to stay for other locations that will be included in our grounding data, such as London, or San Francisco. You'll likely get complete responses about areas or neighborhoods, and some general facts about the city. The playground can use **Web search**, so the answers may cite public websites, but none of them mention Margie's Travel.

### Task 4: Create an AI Assistant with your own data

In this task, you will extract the lab files provided in your VM and attach the Margie's Travel brochures to an agent in the Foundry portal so the model can use them as grounding data.

1. In your VM, open **File Explorer** and navigate to **C:\LabFiles (1)**. Right-click **06-use-own-data.zip (2)** and select **Extract All… (3)**.

   ![](../media/L067.png)

1. In the **Extract Compressed (Zipped) Folders** window, keep the default extraction location **C:\LabFiles\06-use-own-data** and click **Extract (4)**.

   ![](../media/L068.png)

    The extracted folder contains the **data** folder with the brochure PDFs that you use in this task, and the **CSharp** and **Python** applications that you use later in this lab.

1. Navigate back to the Foundry portal. In the **text-turbo** playground, select **Save as agent**.

     ![](../media/mf-8.png)

    >**Note:** If a banner says that you don't have permission to create agents, **Save as agent** shows a lock icon. Select **Save as agent**, and in the **Role assignment required** pop-up, click **Assign me**. When the message **The Foundry User role was assigned** appears, wait about 5 minutes, refresh the browser page, and then select **Save as agent** again.

     ![](../media/mf-9.png)

1. On the **Create an agent** pop-up, replace the pre-filled name with **text-turbo-agent (1)** and click **Create and open playground (2)**.

    ![](../media/mf-10.png)

1. On the agent's **Playground** tab, in the **Instructions** box, replace the existing text with the following instructions. They tell the agent to answer from the brochures that you attach next, instead of from general knowledge or the web.

    ```
    You are a travel assistant for Margie's Travel. Always search the attached brochures with file search and answer from them, citing the brochure you used.
    ```

1. Scroll down the left pane to the **Tools** section and click **Upload files**.

    ![](../media/d-5.png)

1. On the **Attach files** page, click on **Drag and drop files here or browse for files**.

    ![](../media/d-6.png)

1. In the **Open** window, navigate to **C:\LabFiles\06-use-own-data\data (1)**. Select all the brochure PDF files **(2)** and then click **Open (3)** to upload them.

    ![](../media/GM27.png)

1. Once all the files have been uploaded successfully and the status changes to **Success**, click **Attach**.

    ![](../media/d-7.png)

    >**Note:** If any file shows **Failed** with **ERR_BAD_REQUEST: Client error. Failed to get file details.**, the **Foundry User** role assignment from the earlier step hasn't reached every service yet. Click **Cancel**, wait a few minutes, refresh the browser page, and upload the files again.

1. After attaching the files to the agent, click **Save**. The version number next to the agent name increases when the agent is saved.

    >**Note:** If saving fails with **All tool identifiers must be unique across all tool types**, the same file search index was added twice under **Tools**. Select the **ellipsis (...)** on one of the duplicate **File search** entries, select **Remove**, and click **Save** again.

### Task 5: Chat with a model grounded in your data

In this task, you will ask the same questions as before in the chat section after adding your data, and observe how the responses differ.

1. In the Agent **Chat** session on the right side, submit the following queries, and review the responses:

   ```
   I'd like to take a trip to New York. Where should I stay?
   ```

   ![](../media/d-8.png)

   ```
   What are some facts about New York?
   ```

   ![](../media/d-9.png)

1. You'll notice a very different response this time, with specifics about certain hotels and a mention of Margie's Travel, as well as references to where the information provided came from. If you open the PDF reference listed in the response, you'll see the same hotels as the model provided. Try asking it about other cities included in the grounding data, which are Dubai, Las Vegas, London, and San Francisco.

    >**Note:** **Add your data** is still in preview and might not always behave as expected for this feature, such as giving the incorrect reference for a city not included in the grounding data.

### Task 6: Set up an application in Cloud Shell

In this task, you will use a short command-line application running in Cloud Shell on Azure to demonstrate integration with an Azure OpenAI model. Open a new browser tab to access Cloud Shell.

1. In the [Azure portal](https://portal.azure.com?azure-portal=true), select the **[>_] (Cloud Shell)** button at the top of the page to the right of the search box. A Cloud Shell pane will open at the bottom of the portal.

    ![](../media/cshell1.png)

    >**Note:** If you can't find Cloud Shell, click on the **ellipsis (...) (1)** and then select **Cloud Shell (2)** from the menu.

    ![](../media/180625(14)1.png)

1. The first time you open the Cloud Shell, you may be prompted to choose the type of shell you want to use (*Bash* or *PowerShell*). Select **Bash**. If you don't see this option, skip the step.

     ![](../media/bash11.png)

1. Within the **Getting started** page, select **Mount storage account (1)**, select your **Subscription (2)** from the dropdown and click **Apply (3)**.

     ![](../media/lab3-02-41.png)

1. Within the **Mount storage account** page, select **I want to create a storage account (1)** and click **Next (2)**.

    ![](../media/csanext1.png)

1. Within the **Create storage account** page, enter the following details:

    - **Subscription:** Default - Pre-assigned subscription **(1)**.
    - **Resource group:** **openai-<inject key="DeploymentID" enableCopy="false"></inject> (2)**
    - **Region:** Select **<inject key="Region" enableCopy="false" /> (3)**
    - **Storage account name:** **stg<inject key="DeploymentID" enableCopy="false"></inject> (4)**
    - **File share:** enter **none** **(5)**
    - Click **Create (6)**

       ![](../media/l5-12-st1.png)

1. On the Cloud Shell toolbar, select **Manage files (1)** and then select **Upload (2)**.

1. In the **Open** window, navigate to **C:\LabFiles**, select **06-use-own-data.zip** and click **Open**. Wait for the notification **Successfully uploaded a file** at the bottom right of the Cloud Shell pane. It shows the destination **/home/odl_user**, your Cloud Shell home directory.

1. In the Cloud Shell pane, enter the following commands to extract the lab files and navigate to the folder containing the application code files.

    ```bash
    cd ~
    rm -rf 06-use-own-data
    unzip -o 06-use-own-data.zip -d 06-use-own-data
    cd 06-use-own-data
    ```

    This folder contains everything you need for the lab, ready to use:

    - **CSharp** and **Python**: the same application in both languages, already written for the latest Azure OpenAI API.
    - **data**: the Margie's Travel brochure PDFs that the application uses as its grounding data.

1. Use the following command to open the lab files in the built-in code editor.

    ```bash
    code .
    ```

    >**Note:** If the editor's file list stays empty, or you see **Unable to open ... in editor**, the Cloud Shell editor service is not responding. You can still complete the lab in the terminal: open each file with `nano <file>` instead, for example `nano Python/.env`. To save in nano, press **Ctrl+O**, then **Enter**, and press **Ctrl+X** to exit.

### Task 7: Configure your application

In this task, you will add your Azure OpenAI connection details to the application and review how it grounds the model in your data. You don't need to change any code.

1. In the code editor, expand the folder for your preferred language (**CSharp** or **Python**) and open its configuration file:

    - **C#**: `appsettings.json`

        ```json
        {
          "AzureOAIEndpoint": "Your OpenAI endpoint",
          "AzureOAIKey": "Azure OpenAI Key",
          "AzureOAIDeploymentName": "text-turbo",
          "VectorStoreId": ""
        }
        ```

    - **Python**: `.env`

        ```
        AZURE_OAI_ENDPOINT=<Your OpenAI endpoint>
        AZURE_OAI_KEY=<Azure OpenAI Key>
        AZURE_OAI_DEPLOYMENT=text-turbo
        VECTOR_STORE_ID=
        ```

1. Update the configuration file with the following values:

    - **Azure OpenAI endpoint**: Paste the endpoint URL you copied in Task 1 (from the **Keys and Endpoint** page of your Foundry resource).
    - **Azure OpenAI key**: Paste **Key 1** you copied in Task 1.
    - **Deployment name**: Keep **text-turbo**, the model deployment you created in Task 2.
    - **Vector store ID**: Leave this value empty. The first time you run the application, it uploads the brochures and creates a vector store for you.

1. Press **Ctrl+S** to save the file.

    ![](../media/d-3.png)

    ![](../media/d-4.png)

1. Open the code file for your language, **CSharp/OwnData.cs** or **Python/ownData.py**, and review the code. It does three things:

    - **Builds the knowledge base:** If no vector store ID is configured, it uploads every PDF in the `data` folder and adds them to a new vector store.
    - **Grounds the model:** It sends your question to the model using the Responses API with the **file search** tool, and its instructions tell the model to answer only from the brochures.
    - **Shows its sources:** It prints the answer, followed by each brochure file the answer cited.

### Task 8: Run your application

In this task, you will run the application, ask a question, and see an answer grounded in the Margie's Travel brochures, together with the files it came from.

1. In the Cloud Shell terminal, follow the steps for your preferred language.

    **For C#:**

    1. Navigate to the **CSharp** folder.

        ```bash
        cd CSharp
        ```

    1. Run the application. Cloud Shell already includes the .NET SDK, so there is nothing to install. The first run restores the required NuGet packages, which are already listed in `CSharp.csproj`, and can take a minute.

        ```bash
        dotnet run
        ```

        >**Note:** If you see the warning **An issue was encountered verifying workloads**, you can ignore it. It doesn't affect this application.

    **For Python:**

    1. Navigate to the **Python** folder, create and activate a virtual environment, and install the required packages.

        ```bash
        cd Python
        python -m venv labenv
        source labenv/bin/activate
        pip install python-dotenv openai==3.52.0
        ```

    1. Run the application.

        ```bash
        python ownData.py
        ```

1. On the first run, the application uploads the six brochures and creates a vector store. This takes about a minute. When it finishes, it prints a **Vector store ID**.

1. When prompted, enter the following question:

    ```
    Tell me about London
    ```

1. Review the response. It should describe London using details from the Margie's Travel brochures and list the brochure files it used, for example **Source: London Brochure.pdf**.

    ![](../media/optown.png)

1. **(Optional)** Copy the **Vector store ID** printed on the first run and paste it as the **VectorStoreId** (C#) or **VECTOR_STORE_ID** (Python) value in your configuration file, then save the file. Run the application again and ask about another city in the grounding data, such as Dubai, Las Vegas, New York or San Francisco. This time it reuses your existing vector store instead of uploading the brochures again.

## Summary

In this lab, you have accomplished the following:
-   Provisioned a Microsoft Foundry resource
-   Deployed an OpenAI model within the Microsoft Foundry portal
-   Grounded an agent in your own data in the Foundry portal and compared its responses with the base model
-   Ran an application that uses the Responses API and file search to answer questions from your own data

### You have successfully completed the lab.
