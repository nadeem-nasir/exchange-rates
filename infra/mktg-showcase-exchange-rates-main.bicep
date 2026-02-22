
@description('Location to deploy the resources')
param location string = resourceGroup().location

@description('Prefix for the environment name when creating the resources in this deployment.')
param environmentNamePrefix string

@description('Prefix for the name of the application, workload, or service that the resource is a part of.')
param ProjectOrApplicationNamePrefix string = 'showcase'

@description('Prefix for the name of the application instance.')
param ProjectOrApplicationInstanceNamePrefix string = '01'

@description('Runtime for the function worker.')
@allowed([
  'dotnet'
  'node'
  'python'
  'java'
  'dotnet-isolated'
])
param functionWorkerRuntime string 

@description('Name of the function app.')
param FunctionAppName string

@description('Specifies the operating system used for the Azure Function hosting plan.')
@allowed([
  'Windows'
  'Linux'
])
param functionPlanOS string


var applicationRegionPrefix = substring(location, 0, 2)

var resourceNameSuffix = '${environmentNamePrefix}-${applicationRegionPrefix}-${ProjectOrApplicationInstanceNamePrefix}'

var hostingPlanName = 'asp-${ProjectOrApplicationNamePrefix}-${resourceNameSuffix}'

var storageAccountNamePrefix = 'st-${ProjectOrApplicationNamePrefix}-${resourceNameSuffix}'

var storageAccountFullName = toLower(replace(storageAccountNamePrefix, '-', ''))

var storageAccountName = length(storageAccountFullName) > 23 ? substring(storageAccountFullName,0,23) : storageAccountFullName

var isReserved = ((functionPlanOS == 'Linux') ? true : false)

var functionAppkind = (isReserved ? 'functionapp,linux' : 'functionapp')


var functionAppName = 'func-${ProjectOrApplicationNamePrefix}-${FunctionAppName}-${resourceNameSuffix}'


module storageAccountResource './templates/storage-account.bicep' ={
  name: storageAccountName
  params: { 
  storageAccountName:storageAccountName
  storageAccountType:'Standard_LRS'
  location:location
  }
}

module containersBlobServices './templates/blob-services.bicep' ={
  name:'containersBlobServices'
  params:{
    storageAccountName:storageAccountName
    storagecontainersNames:[
      'test-container'
      'ingest-daily-exchange-rates-blob-trigger'
    ]
  }
}

module queueNamesServices './templates/queue-services.bicep' ={
  name:'queueServices'
  params:{
    storageAccountName:storageAccountName
    queueNames:[
      'test-queue'
      'ingest-daily-exchange-rates-queue-trigger'
    ]
  }
}

module HostingPlan './templates/hosting-plan.bicep' ={
  name: hostingPlanName
  params: { 
    location:location
    hostingPlanName:hostingPlanName
    isReserved:isReserved
  }
  dependsOn: [
    storageAccountResource
  ]
}


module FunctionAppExchangeRatesProcessing './templates/function-app.bicep'={
  name: functionAppName
  params: {
    functionAppName:functionAppName
    hostingPlanId:HostingPlan.outputs.hostingPlanId
    storageAccountName:storageAccountResource.name
    functionWorkerRuntime:functionWorkerRuntime
    location:location
    netFrameworkVersion:'8.0'
    linuxFxVersion:'DOTNET-ISOLATED|8.0'
    kind:functionAppkind
    applicationInsightsInstrumentationKey:'8f0c7426-8512-41c5-8d84-2270e70a6d9b'
    additionalAppSettings:{
    ExchangeRatesAPIAccesskey:''
    }  
   }
   dependsOn: [
    HostingPlan
    storageAccountResource    
  ]
}




