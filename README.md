# DotNet Azure Functions Demo

This repository contains a demo project for Azure Functions using .NET 8.0. The project includes an HTTP trigger function, infrastructure deployment scripts, and configuration files.

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio%2Cblob-storage) to emulate my storage account locally

## Tech I'm using

- [Bicep Azure Verified Modules](https://azure.github.io/Azure-Verified-Modules/indexes/bicep/) to implement the IaC to support everything I need to demo this.
- [Github Actions](https://docs.github.com/en/actions/about-github-actions/understanding-github-actions) - Automatically deploys infrastructure and code on push to main. See [GitHub Actions CI/CD](#github-actions-cicd) section for setup.
- *Coming Soon* [Managed Identity for Storage](https://learn.microsoft.com/en-us/azure/azure-functions/functions-identity-based-connections-tutorial) - Use managed identity for storage account connection instead of connection string authentication.

### Building the Project

To build the project, run the following command:

```sh
dotnet build
```

### Running the Project Locally

To run the project locally, use the Azure Functions Core Tools:

```sh
func start
```

### Deploying the Project to Azure

#### Option 1: GitHub Actions CI/CD

The recommended way to deploy is using the GitHub Actions workflow. See [GitHub Actions CI/CD](#github-actions-cicd) section below for setup instructions.

#### Option 2: Manual Deployment Script

I have created a generic `deploy.sh` that can be used to deploy the project to azure by creating your own `deploy.<env>.sh` file.  The simplest thing to do is to copy the `deploy.sh` to a new file based on the environment you are working with, so something like `deploy.sandbox.sh` and then replace all the values that are enclosed in `<angle-brackets>`.

This script will run the [Bicep modules](https://github.com/anotherRedbeard/dotnet-azfunc-demo/blob/main/iac/bicep) needed to create all the required resources to run this example and it will deploy the code to azure using the `azure functionapp publish` cli command.  It supports three options: all, infra, and function.

```sh
./deploy.sh all|infra|function
```

- `all`: Deploys both the infrastructure and the function.
- `infra`: Deploys only the infrastructure.
- `function`: Deploys only the function.

### Configuration

- `host.json`: Contains the configuration for the Azure Function host.
- `local.settings.json`: Contains local settings for the Azure Functions project.
  - Here is an example `local.settings.json` file that you can use since it is in the `.gitignore` file so it won't be included
  
    ```json
    {
        "IsEncrypted": false,
        "Values": {
            "AzureWebJobsStorage": "",
            "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
        }
    }
    ```

### Logging

The logging configuration is set in `Program.cs`.  This is to represent how you can override the [default behavior of Application Insights SDK](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows#managing-log-levels) inside of the isolated worker process. By default it instructs the logger to capture only warnings and more sever logs. The code in the `Program.cs` shows you how you can remove that filter rule and add your own that is set to `Trace` for more detailed logging.

### HTTP Triggered Function

Here I am just using the base code with all the different levels of log messages to show they are displayed in both the console output and the trace table in Application Insights.

### Setting up secure webhook for Azure Monitor ActionGroup

I have created a [powershell script](./secureWebhookSetup.ps1) that will setup the app role and `Azns AAD Webhook` Service Principal and assign the role to it so you can setup a secure webhook for your Azure Monitor ActionGroup.  To see the full how-to, please check out my blog post [here](https://techcommunity.microsoft.com/blog/healthcareandlifesciencesblog/setting-up-a-secure-webhook-in-an-azure-monitor-action-group/4384445).

## GitHub Actions CI/CD

The project includes a GitHub Actions workflow for automated deployment to Azure. This follows Microsoft best practices for deploying .NET isolated Azure Functions to Linux Elastic Premium plans.

### How It Works

The workflow uses `az functionapp deployment source config-zip` for zip deployment with `WEBSITE_RUN_FROM_PACKAGE=1`. This is the [recommended approach](https://learn.microsoft.com/en-us/azure/azure-functions/functions-deployment-technologies) for Premium plans on Linux because:

1. The zip package is uploaded to `/home/data/SitePackages`
2. The package is mounted read-only at runtime (better performance)
3. No blob storage URL or SAS tokens are required

### Required GitHub Secrets

Configure these secrets in your GitHub repository settings (`Settings > Secrets and variables > Actions`):

| Secret | Description |
|--------|-------------|
| `AZURE_CREDENTIALS` | Service principal credentials JSON (see below) |
| `AZURE_SUBSCRIPTION_ID` | Your Azure subscription ID |

### Creating the Service Principal

Run this Azure CLI command to create a service principal with Contributor access:

```bash
# Replace with your subscription ID and resource group
az ad sp create-for-rbac \
  --name "github-azfunc-demo-sp" \
  --role contributor \
  --scopes /subscriptions/{subscription-id} \
  --sdk-auth
```

Copy the entire JSON output and save it as the `AZURE_CREDENTIALS` secret.

### Triggering Deployment

The workflow runs automatically on:
- Push to `main` branch
- Manual trigger via GitHub Actions UI (workflow_dispatch)

### Workflow Steps

1. **Build**: Restores, builds, and publishes the .NET project
2. **Deploy Infrastructure**: Deploys Bicep templates via ARM deployment
3. **Deploy Function**: 
   - Sets `WEBSITE_RUN_FROM_PACKAGE=1`
   - Uploads zip package via `config-zip`
   - Syncs function triggers
   - Verifies function registration

### License

This project is licensed under the MIT License. See the `LICENSE` file for details.