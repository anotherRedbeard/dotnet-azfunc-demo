# Bicep

This directory contains bicep templates that are used to create the function app resources so I can deploy my dotnetcore azure function. If you want to use your own parameter files, this repo is setup to copy the `*.bicepparam` files and create your own with the `dev.bicepparam` extension. They will be ignored from check-in. I'm using [Azure Verifed Modules](https://azure.github.io/Azure-Verified-Modules/indexes/bicep/bicep-resource-modules/) where they exist to create these bicep files.

## Deploying with Bicep

Bicep is an Infrastructure as Code (IaC) language developed by Microsoft for deploying Azure resources in a declarative manner. It simplifies the deployment process and enhances readability and maintainability of your infrastructure code. Here is the [official Bicep documentation](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/)

### Prerequisites

Before you begin, ensure you have the following installed:

- Azure CLI: Bicep is integrated directly into the Azure CLI and provides first-class support for deploying Bicep files.
- Bicep CLI: While not strictly necessary due to Azure CLI integration, the Bicep CLI can be useful for compiling, decompiling, and validating Bicep files.

### Steps to Deploy

1. **Login to Azure**

    Start by logging into Azure with the Azure CLI:

    ```bash
    az login
    ```

2. **Set your subscription**

    Make sure you're working with the correct Azure subscription:

    ```bash
    az account set --subscription "<Your-Subscription-ID>"
    ```

3. **Create Resource Group (Optional)

    Make sure the resource group has already been created:

    ```base
    az group create --name <resource-group-name> --location <location>
    ```

4. **Compile Bicep file (Optional)

    If you have Bicep CLI installed, you can manually compile your Bicep file to an ARM template. This step is optional because Azure CLI compiles Bicep files automatically on deployment.

    ```bash
    bicep build <your-file>.bicep
    ```

5. **Deploy the Bicep file**

    Use the Azure CLI to deploy your Bicep file. Replace `<subscription-id>` with your subscription Id, `<location>` with the location you want to deploy this to, and `<your-deployment-name>` with a name for your deployment.  **Note**: since we are using bicep parameter files and they are tied to one bicep file we don't need the --template-file switch.  See [Bicep file with parameters file](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/parameter-files?tabs=Bicep#deploy-bicep-file-with-parameters-file) for more info.

    ```bash
    az deployment sub create --subscription <subscription-id> --location <location> --name <your-deployment-name> --parameters <your-file>.bicepparam
    ```

### Deploying examples in this repo

1. **Base Function resources to support the code from this repo**

    Prerequisites:
    - None, everything should get created using the bicep file

    `create-dotnet-function-all.bicep` is the main template for this.

    What's included:

    - Resource Group
    - App Service Plan - Linux
    - Log Analytics Workspace
    - Application Insights
    - Function App

    **Command to deploy via bicep:**

    ```bash
    az deployment sub create --subscription <subscription-id> --location <location> --name dotnetcore-azfunction-deploy --parameters ./iac/bicep/create-dotnet-function-all.dev.bicepparam
    ```