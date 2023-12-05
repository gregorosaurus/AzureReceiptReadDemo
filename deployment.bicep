param environmentName string = 'demo'

param location string = 'canadacentral'
param locationShortCode string = 'cc'

@description('The prefix that is inserted into resource names to give it uniqueness')
param resourcePrefix string = 'gm'

@description('The SQL password for the sql database.  Note: in production, use Entra ID for authentication')
@secure()
param sqlAdminPassword string = 'GiraffesAreReallyTall1!'

@description('Username of primary user fot sftp')
param sftpUsername string = 'receipts'

@description('Home directory of primary user. Should be a container.')
param sftpHomeDirectory string = 'receipts'

@description('SSH Public Key for primary user. If not specified, Azure will generate a password which can be accessed securely')
param sftpPublicKey string = ''

resource receiptSqlDbServer 'Microsoft.Sql/servers@2022-05-01-preview' ={
  name: 'sqlsrv-${resourcePrefix}azrcptdemo-${environmentName}-${locationShortCode}'
  location: location
  properties:{
    administratorLogin: 'receiptSqlAdmin'
    administratorLoginPassword: sqlAdminPassword 
  }
}

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: receiptSqlDbServer
  name: 'sqldb-${resourcePrefix}azrcptdemo-${environmentName}-${locationShortCode}'
  location: location
  sku: {
      name: 'Basic'
      tier: 'Basic'
      capacity:  5
  }
  properties:{
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    autoPauseDelay: -1
    maxSizeBytes: 2147483648 
    requestedBackupStorageRedundancy: 'Local'
  }
}

resource functionStorageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'stfn${resourcePrefix}azrcptdemo${environmentName}${locationShortCode}'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true //needed for function app
    isHnsEnabled: false //used for the application - HNS not required
    accessTier: 'Hot'
    supportsHttpsTrafficOnly: true
  }
}

resource sftpStorageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'stsftp${resourcePrefix}azrcptdemo${environmentName}${locationShortCode}'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true //needed for function app, in prod, connect with MSI
    isHnsEnabled: true //needed for sftp
    accessTier: 'Hot'
    supportsHttpsTrafficOnly: true
    isLocalUserEnabled: true
    isSftpEnabled: true
  }
}

resource sftpContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  name: '${sftpStorageAccount.name}/default/${sftpHomeDirectory}'
  properties: {
    publicAccess: 'None'
  }
}

resource sftpUser 'Microsoft.Storage/storageAccounts/localUsers@2021-04-01' = {
  parent: sftpStorageAccount
  name: sftpUsername
  properties: {
    permissionScopes: [
      {
        permissions: 'rcwdl'
        service: 'blob'
        resourceName: sftpHomeDirectory
      }
    ]
    homeDirectory: sftpHomeDirectory
    sshAuthorizedKeys: empty(sftpPublicKey) ? null : [
      {
        description: '${sftpUsername} public key'
        key: sftpPublicKey
      }
    ]
    hasSharedKey: false
  }
}


///Function 

resource dataProcessorFunctionAppHostingPlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: 'asp-${resourcePrefix}azrcptdemo-${environmentName}-${locationShortCode}'
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
    size: 'Y1'
    family: 'Y'
    capacity: 0
  }
  properties: {
    computeMode: 'Dynamic'
  }
}

resource dataProcessorFunction 'Microsoft.Web/sites@2022-03-01' = {
  name: 'fn-${resourcePrefix}azrcptdemo-${environmentName}-${locationShortCode}'
  location: location
  kind: 'functionapp'
  identity:{
    type:'SystemAssigned'    
  }
  properties: {
    serverFarmId: dataProcessorFunctionAppHostingPlan.id
    siteConfig: {
      netFrameworkVersion:'v6.0'
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${functionStorageAccount.name};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${functionStorageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${functionStorageAccount.name};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${functionStorageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: 'func-${resourcePrefix}azrcptdemo-${environmentName}-${locationShortCode}'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'SFTPStorageConnectionString'
          value: 'DefaultEndpointsProtocol=https;AccountName=${sftpStorageAccount.name};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${sftpStorageAccount.listKeys().keys[0].value}'
        }
      ]
    }
  }
}

///
//form recognizer

resource cognitiveService 'Microsoft.CognitiveServices/accounts@2021-10-01' = {
  name:  'di-${resourcePrefix}azrcptdemo-${environmentName}-${locationShortCode}'
  location: location
  sku: {
    name: 'S0'
  }
  kind: 'FormRecognizer'
  properties: {
    apiProperties: {
      statisticsEnabled: false
    }
  }
}
