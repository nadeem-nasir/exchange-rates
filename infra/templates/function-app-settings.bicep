@description('The name of the function app.')
param functionAppName string 

param functionAppSettings object ={}
param additionalAppSettings object ={}
param currentAppSettings object ={}

resource functionApp 'Microsoft.Web/sites@2021-03-01' existing = {
  name: functionAppName
}

resource siteconfig 'Microsoft.Web/sites/config@2021-03-01' = {
  parent: functionApp
  name: 'appsettings'
  properties: union(functionAppSettings, additionalAppSettings,currentAppSettings)
}

