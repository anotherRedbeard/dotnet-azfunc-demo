# DotNet Azure Functions Demo

This repository contains a demo project for Azure Functions using .NET 8.0. The project includes an HTTP trigger function, infrastructure deployment scripts, and configuration files.

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)

## Tech I'm using

- [bicep](https://github.com/anotherRedbeard/apimdemo-environment/tree/main/iac/bicep) to implement the GenAI features as well as general infrastructure to support everything else.
- *Coming Soon* [Github Actions](https://docs.github.com/en/actions/about-github-actions/understanding-github-actions) - This will be used to automatically deploy everything on commit instead of having to manually run the `deploy.<env>.sh` script.

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

### License

This project is licensed under the MIT License. See the `LICENSE` file for details.