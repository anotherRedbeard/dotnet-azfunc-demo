targetScope = 'subscription'

@description('Provide the id of the subscription.')
param subscriptionId string = 'subscriptionId'

@description('Provide the name of the resource group.')
param rgName string = 'rgName'

@description('Provide the location of the apim instance.')
param location string = 'location'

@description('Provide the name of the app service plan.')
param aspName string = 'App Service Plan name'

@allowed([
  'EP1'
  'EP2'
  'EP3'
])
@description('The name of the app service plan sku.')
param sku string = 'EP1'

@description('Name of the log analytics workspace.')
param lawName string = '<lawName>'

@description('Name of the app insights resource.')
param appInsightsName string = '<appInsightsName>'

@description('Name of the storage account to map to function app.')
param storageAccountName string = '<storageAccountName>'

@description('Name of the function app.')
param functionAppName string = '<functionApp>'

//create resource group
module resourceGroupResource 'br/public:avm/res/resources/resource-group:0.4.3' = {
  name: 'createResourceGroup'
  scope: subscription(subscriptionId)
  params: {
    name: rgName
    location: location
  }
}

//app service plan
module serverfarm 'br/public:avm/res/web/serverfarm:0.7.0' = {
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  name: 'serverfarmDeployment'
  params: {
    // Required parameters
    name: aspName
    skuCapacity: 1
    skuName: sku
    // Non-required parameters
    kind: 'Elastic'
    maximumElasticWorkerCount: 2
    reserved: true
    location: location 
    perSiteScaling: false
    zoneRedundant: false
  }
}

//log analytics workspace resource
module workspace 'br/public:avm/res/operational-insights/workspace:0.15.0' = {
  name: 'workspaceDeployment'
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  params: {
    // Required parameters
    name: lawName
    // Non-required parameters
    location: location
  }
}

//app insights resource
module component 'br/public:avm/res/insights/component:0.7.1' = {
  name: 'componentDeployment'
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  params: {
    // Required parameters
    name: appInsightsName
    workspaceResourceId: workspace.outputs.resourceId
    // Non-required parameters
    location: location
  }
}

//create storage account
module storageAccount 'br/public:avm/res/storage/storage-account:0.32.0' = {
  name: 'storageAccountDeployment'
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  params: {
    // Required parameters
    name: storageAccountName
    // Non-required parameters
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true  // Required for connection string auth to work
    location: location
    skuName: 'Standard_LRS'
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
      ipRules: [ ]
      virtualNetworkRules: [ ]
    }
  }
}

//create function app
module site 'br/public:avm/res/web/site:0.22.0' = {
  name: 'siteDeployment'
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  params: {
    // Required parameters
    kind: 'functionapp,linux'
    name: functionAppName
    serverFarmResourceId: serverfarm.outputs.resourceId
    // Non-required parameters
    siteConfig: {
      numberOfWorkers: 1
      linuxFxVersion: 'DOTNET-ISOLATED|8.0'
      alwaysOn: false
      use32BitWorkerProcess: false
      minimumElasticInstanceCount: 1
    }
    location: location
    configs: [
      {
        name: 'appsettings'
        applicationInsightResourceId: component.outputs.resourceId
        storageAccountResourceId: storageAccount.outputs.resourceId
        storageAccountUseIdentityAuthentication: false
        properties: {
          AzureFunctionsJobHost__logging__logLevel__default: 'Warning'
          FUNCTIONS_EXTENSION_VERSION: '~4'
          FUNCTIONS_WORKER_RUNTIME: 'dotnet-isolated'
          // For Linux Elastic Premium, WEBSITE_RUN_FROM_PACKAGE=1 is recommended
          // This allows zip deployment to /home/data/SitePackages
          // Per: https://learn.microsoft.com/en-us/azure/azure-functions/run-functions-from-deployment-package
          WEBSITE_RUN_FROM_PACKAGE: '1'
        }
      }
    ]
  }
}
