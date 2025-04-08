@description('The location used for all deployed resources')
param location string = resourceGroup().location

@description('Tags that will be applied to all resources')
param tags object = {}


param weatherFunctionAppExists bool

@description('Id of the user or app to assign application roles')
param principalId string

var abbrs = loadJsonContent('./abbreviations.json')
var resourceToken = uniqueString(subscription().id, resourceGroup().id, location)

@secure()
@description('The password for the jumpbox VM admin user')
param adminPassword string

module virtualNet './modules/createnetwork.bicep' = {
  name: 'internal-network'
  params: {    
    resourceToken: resourceToken    
  }
}


// Monitor application with Azure Monitor
module monitoring 'br/public:avm/ptn/azd/monitoring:0.1.0' = {
  name: 'monitoring'
  params: {
    logAnalyticsName: '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
    applicationInsightsName: '${abbrs.insightsComponents}${resourceToken}'
    applicationInsightsDashboardName: '${abbrs.portalDashboards}${resourceToken}'
    location: location
    tags: tags
  }
}

// Container registry
module containerRegistry 'br/public:avm/res/container-registry/registry:0.1.1' = {
  name: 'registry'
  params: {
    name: '${abbrs.containerRegistryRegistries}${resourceToken}'
    location: location
    tags: tags
    publicNetworkAccess: 'Enabled'
    roleAssignments:[
      {
        principalId: weatherFunctionAppIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
      }
    ]
  }
}

// Container apps environment
module containerAppsEnvironment 'br/public:avm/res/app/managed-environment:0.5.1' = {
  name: 'container-apps-environment'
  params: {
    logAnalyticsWorkspaceResourceId: monitoring.outputs.logAnalyticsWorkspaceResourceId
    name: '${abbrs.appManagedEnvironments}${resourceToken}'
    location: location
    zoneRedundant: false    
    internal: true
    infrastructureSubnetId: virtualNet.outputs.subnetId 
    workloadProfiles: [
      {
        maximumCount: 3
        minimumCount: 0
        name: 'profile01'
        workloadProfileType: 'D4'
      }
    ]
  }
}

module weatherFunctionAppIdentity 'br/public:avm/res/managed-identity/user-assigned-identity:0.2.1' = {
  name: 'weatherFunctionAppidentity'
  params: {
    name: '${abbrs.managedIdentityUserAssignedIdentities}weatherFunctionApp-${resourceToken}'
    location: location
  }
}

module weatherFunctionAppFetchLatestImage './modules/fetch-container-image.bicep' = {
  name: 'weatherFunctionApp-fetch-image'
  params: {
    exists: weatherFunctionAppExists
    name: 'weather-function-app'
  }
}

module weatherFunctionApp 'br/public:avm/res/app/container-app:0.8.0' = {
  name: 'weatherFunctionApp'
  params: {
    name: 'weather-function-app'
    ingressTargetPort: 80
    scaleMinReplicas: 1
    scaleMaxReplicas: 10
    secrets: {
      secureList:  [
      ]
    }
    containers: [
      {
        image: weatherFunctionAppFetchLatestImage.outputs.?containers[?0].?image ?? 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
        name: 'main'
        resources: {
          cpu: json('0.5')
          memory: '1.0Gi'
        }
        env: [
          {
            name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
            value: monitoring.outputs.applicationInsightsConnectionString
          }
          {
            name: 'AZURE_CLIENT_ID'
            value: weatherFunctionAppIdentity.outputs.clientId
          }
          {
            name: 'PORT'
            value: '80'
          }
        ]
      }
    ]
    managedIdentities:{
      systemAssigned: false
      userAssignedResourceIds: [weatherFunctionAppIdentity.outputs.resourceId]
    }
    registries:[
      {
        server: containerRegistry.outputs.loginServer
        identity: weatherFunctionAppIdentity.outputs.resourceId
      }
    ]
    environmentResourceId: containerAppsEnvironment.outputs.resourceId
    location: location
    tags: union(tags, { 'azd-service-name': 'weather-function-app' })
  }
}

// resource applicationGateway 'Microsoft.Network/applicationGateways@2020-11-01' = {
//     name: 'weather-gateway'
//     location: location    
//     properties: {
//         sku: {
//             name: 'Standard_v2'
//             tier: 'Standard_v2'
//             capacity: 2
//         }       
//         gatewayIPConfigurations: [
//             {
//                 name: 'appGatewayIpConfig'
//                 properties: {
//                     subnet: {
//                         id: virtualNet.outputs.gatewaySubnetId
//                     }
//                 }
//             }
//         ]
//         frontendIPConfigurations: [
//             {
//                 name: 'appGatewayFrontendIP'
//                 properties: {
//                     publicIPAddress: {
//                         id: virtualNet.outputs.gatewayPip
//                     }
//                 }
//             }
//         ]
//         frontendPorts: [
//             {
//                 name: 'appGatewayFrontendPort'
//                 properties: {
//                     port: 80
//                 }
//             }
//         ]
//         backendAddressPools: [
//             {
//                 name: 'appGatewayBackendPool'
//                 properties: { 
//                     backendAddresses: [
//                         {
//                             fqdn: weatherFunctionApp.outputs.fqdn
//                         }                        
//                     ]
//                 }
//             }
//         ]
//         backendHttpSettingsCollection: [
//             {
//                 name: 'appGatewayBackendHttpSettings'
//                 properties: {
//                     port: 80
//                     protocol: 'Http'
//                     cookieBasedAffinity: 'Disabled'
//                     requestTimeout: 20
//                     pickHostNameFromBackendAddress: true // Use the host header from the backend target
//                 }
//             }
//         ]
//         httpListeners: [
//             {
//                 name: 'appGatewayHttpListener'
//                 properties: {
//                     frontendIPConfiguration: {
//                         id: resourceId('Microsoft.Network/applicationGateways/frontendIPConfigurations', 'weather-gateway', 'appGatewayFrontendIP')
//                     }
//                     frontendPort: {
//                         id: resourceId('Microsoft.Network/applicationGateways/frontendPorts', 'weather-gateway', 'appGatewayFrontendPort')
//                     }
//                     protocol: 'Http'
//                 }
//             }
//         ]
//         requestRoutingRules: [
//             {
//                 name: 'appGatewayRoutingRule'
//                 properties: {
//                     ruleType: 'Basic'
//                     httpListener: {
//                         id: resourceId('Microsoft.Network/applicationGateways/httpListeners', 'weather-gateway', 'appGatewayHttpListener')
//                     }
//                     backendAddressPool: {
//                         id: resourceId('Microsoft.Network/applicationGateways/backendAddressPools', 'weather-gateway', 'appGatewayBackendPool')
//                     }
//                     backendHttpSettings: {
//                         id: resourceId('Microsoft.Network/applicationGateways/backendHttpSettingsCollection', 'weather-gateway', 'appGatewayBackendHttpSettings')
//                     }
//                 }
//             }
//         ]
//     }
// }

resource dnsZone 'Microsoft.Network/privateDnsZones@2024-06-01' = {
  name: 'privatelink.${location}.azurecontainerapps.io'
  location: 'global'
  properties: {
  }
}

resource privateLinksZone 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2024-06-01' = {
  parent: dnsZone
  name: '4aeb9b18b0f36'
  location: 'global'
  properties: {
    registrationEnabled: false
    resolutionPolicy: 'Default'
    virtualNetwork: {
      id: virtualNet.outputs.virtualNetworkId
    }
  }
}

module jumpboxModule './jumpbox.bicep' = {
  name: 'jumpboxDeployment'
 
  params: { 
    jumpboxVmSubnetId: virtualNet.outputs.defaultSubnetId
    adminUserName: 'robert'
    adminPassword: adminPassword
  }
}

output AZURE_CONTAINER_REGISTRY_ENDPOINT string = containerRegistry.outputs.loginServer
output AZURE_RESOURCE_WEATHER_FUNCTION_APP_ID string = weatherFunctionApp.outputs.resourceId
// output defaultDomain string = containerAppsEnvironment.outpproperties.defaultDomain
// output containerAppStaticIp string = containerAppsEnvironment.properties.staticIp