# Title
How to Ingest and Sync Daily Exchange Rates with Azure Durable Functions in the .NET Isolated Worker
# Introduction 
This case study demonstrates how to use Azure Durable Functions in the .NET isolated worker to implement a fan out/fan in pattern to ingest daily exchange rates from an API URL and store them in a table storage.
It also shows how to use different triggers, such as TimerTrigger, HttpTrigger, QueueTrigger, and BlobTrigger, to start the downloading process. 
Finally, it shows how to use the fan out/fan in pattern to sync the exchange rates to different ERP systems in parallel.
# Preqrequisites
- Azure subscription
- Visual Studio 2022 or later 
- .NET 8.0
- Azure Functions Core Tools v4.x
- Azure development workload in Visual Studio
- Azure storage account
- Signup for exchangeratesapi.io and get the API key and enter the API key in environment varialbe or local.settings.json in ExchangeRatesAPIAccesskey file
# Azure resources used in the project
- Azure Function App in .NET 8.0, isolated worker process  Linux OS, Durable Functions
- Azure hosting plan
- Azure Confugurations in Bicep template
- Azure Storage
- Azure blob storage
- Table storage
- Create a container using bicep template and blob service 
- Create queue names using bicep template
- Azure Storage account role assignment in Bicep template
# To run the application locally
- Clone the repository
- Open the solution in Visual Studio 2022 or later
- Rename template.local.settings.json to local.settings.json
- Update the connection string in local.settings.json or in appsettings.json
- Build the solutio and wait to restore packages
- Bicep template deploy all the required resources and also set the required configurations. so it is easy to deploy the infrastructure and test the application
# Deploying Azure Function Apps using Azure DevOps
- Azure subscription
- Deploy infrastructure using Azure Bicep temnplate from infra folder
- Create a new project in Azure DevOps or GitHub
- Create a new service connection in Azure DevOps or GitHub
- Create variables group in Azure DevOps or GitHub
- Create a new pipeline in Azure DevOps or GitHub using the pipelines.yaml file from deploy folder
- Push the code to the repository
# Learning Outcomes of the Case Study 
- Understand the concept and benefits of the fan out/fan in pattern for parallel processing of data
- Learn how to use Azure Durable Functions in the .NET isolated worker to create stateful and scalable workflows
- Learn how to use different triggers to start the downloading of exchange rates from an API URL
- Learn how to use HttpClient in service, global usings, and extension methods to simplify the code
- Learn how to use Table Storage Generic Repository implementations to store and retrieve data from table storage
- Learn how to use the fan out/fan in pattern to sync the exchange rates to different ERP systems in parallel
# Various industry use cases of the case study
- Financial services: Sync exchange rates to different accounting, billing, and payment systems
- E-commerce: Update product prices and currency conversions for different markets and regions
- Travel and tourism: Provide accurate and up-to-date exchange rates for travelers and booking platforms
- Education and research: Analyze and compare exchange rate trends and patterns across different sources and time periods
# Extend the Case Study: Todo For Learning
- Deploy the infrastructure using Azure Bicep
- Change the AzureWebJobsStorage and ExchangeRatesAPIAccesskey in the local.settings.json 
- Run the application locally
- Install Azure storage explorer . download  the latest version of Azure Storage Explorer from : https://azure.microsoft.com/en-us/products/storage/storage-explorer
- View the Table Storage in the Azure Storage Explorer 
- Create a new http trigger function that take list of currencies and seed the currencies in the table storage
- To seed the currencies in the table storage, you can use modify SeedTargetCurrencyAsync method 
- Create a new http trigger function that take one currency in http query string and return that currency exchange rate from the table storage. You can use input binding to get the currency exchange rates from the table storage
- Table storage input binding: https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-table-input?tabs=csharp
- Create a new time trigger function that delete old  table from table storage.Currently it is creating new table everyday yyyyMMdd prefix in table name.Maybe keep the table for 7 days and delete the old tables
	
