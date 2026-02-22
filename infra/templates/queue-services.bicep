
@description('A queue name must start with a letter or number, and can only contain letters, numbers, and the dash (-) character. The first and last letters in the queue name must be alphanumeric. The dash (-) character cannot be the first or last character. Consecutive dash characters are not permitted in the queue name.All letters in a queue name must be lowercase.A queue name must be from 3 through 63 characters long.')
param  queueNames array 

@description('Storage account names must be between 3 and 24 characters in length and may contain numbers and lowercase letters only. Your storage account name must be unique within Azure')
@minLength(3)
@maxLength(22)
param storageAccountName string

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' existing = {
  name: storageAccountName
}

resource queueServices 'Microsoft.Storage/storageAccounts/queueServices@2023-01-01' existing = {
  name: 'default'
  parent: storageAccount
}

resource symbolicname 'Microsoft.Storage/storageAccounts/queueServices/queues@2023-01-01' = [for name in range(0, length(queueNames)): {
  name: queueNames[name]
  parent: queueServices
  properties: {
    metadata: {
    }
  }
}]
