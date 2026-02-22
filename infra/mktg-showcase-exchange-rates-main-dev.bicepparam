using 'mktg-showcase-exchange-rates-main.bicep'

@description('This is a prefix used to identify the environment where the resources will be deployed.')
param environmentNamePrefix = 'dev'

@description('This is a prefix used to identify the instance of the CRM (customer relationship management) application.')
param ProjectOrApplicationNamePrefix = 'crmio'

@description('This is a prefix used to identify the CRM (customer relationship management) application.')
param ProjectOrApplicationInstanceNamePrefix = '01'

@description('This is the runtime environment for the function app.')
param functionWorkerRuntime = 'dotnet-isolated'

@description('This is the name of the function app responsible for generating PDFs from HTML.')
param FunctionAppName = 'exchange-rates'

@description('This is the operating system for the function app hosting plan.')
param functionPlanOS = 'Linux'

