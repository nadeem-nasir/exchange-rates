@description('The name of the function app.')
param functionAppName string 

@description('The ID of the hosting plan for the function app.')
param hostingPlanId string 

@description('The name of the storage account for the function app.')
param storageAccountName string 

@description('The location for all resources. Defaults to the location of the resource group.')
param location string = resourceGroup().location

@description('The kind of the function app. Allowed values are "functionapp,linux" and "functionapp".')
@allowed([
  'functionapp,linux'
  'functionapp'
])
param kind string

@description('The language worker runtime to load in the function app. Allowed values are "dotnet", "node", "python", "java", and "dotnet-isolated". Defaults to "dotnet".')
@allowed([
  'dotnet'
  'node'
  'python'
  'java'
  'dotnet-isolated'
])
param functionWorkerRuntime string 

@description('The instrumentation key for Application Insights in the function app.')
@secure()
param applicationInsightsInstrumentationKey string = ''


@description('Additional Function App settings')
param additionalAppSettings object = {}


param netFrameworkVersion string = '8.0'
@description('Required for Linux app to represent runtime stack in the format of \'runtime|runtimeVersion\'. For example: \'python|3.10\'')
param linuxFxVersion string = 'DOTNET-ISOLATED|8.0'


resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' existing ={
  name: storageAccountName
}

resource functionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: functionAppName
  location: location
  kind: kind
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: hostingPlanId    
    siteConfig: {
      linuxFxVersion:linuxFxVersion       
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
      netFrameworkVersion:netFrameworkVersion
      appSettings: [
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
      ]
    }
    httpsOnly: true
  }
  dependsOn: [
    storageAccount
  ]
}

var functionAppSettings ={ 
  AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'  
  FUNCTIONS_EXTENSION_VERSION: '~4'  
  APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsightsInstrumentationKey  
  FUNCTIONS_WORKER_RUNTIME: functionWorkerRuntime  
  WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'  
  WEBSITE_CONTENTSHARE: toLower(functionAppName)
}

  module FunctionAppsettings './function-app-settings.bicep'={ 
    name:'${functionAppName}-appSettings'
    params:{
      functionAppName:functionApp.name
      functionAppSettings:functionAppSettings
      additionalAppSettings:additionalAppSettings           
      currentAppSettings: list(resourceId('Microsoft.Web/sites/config', functionApp.name, 'appsettings'), '2021-03-01').properties
    }
    }

output name string = functionApp.name
output id string = functionApp.id
output identityPrincipalId string = functionApp.identity.principalId
