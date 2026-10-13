// Infrastructure as code: the same Azure resources I built in the portal, described in a file.

param location string = resourceGroup().location
param appName string = 'accesstracker-web'
param sqlServerName string = 'accesstracker-sql'
param sqlDatabaseName string = 'AccessTrackerDb'
param sqlAdminLogin string
@secure()
param sqlAdminPassword string

// The compute plan the web app runs on. F1 = free tier.
resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: '${appName}-plan'
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
  }
}

// The web app itself, running on that plan.
resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: appName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
  }
}

// The logical SQL server that hosts the database.
resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
  }
}

// The database on that server, Basic tier.
resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
}