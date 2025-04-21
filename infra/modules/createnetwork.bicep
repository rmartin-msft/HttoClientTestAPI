@description('The address prefix for the virtual network')
param vnetAddressPrefix string = '10.0.0.0/16'

@description('The address prefix for the subnet')
param subnetAddressPrefix string = '10.0.1.0/24'

@description('The address prefix for the aca integration subnet')
param acaSubnetPrefix string = '10.0.2.0/23'

@description('The address prefix for the gateway integration subnet')
param gatewaySubnetPrefix string = '10.0.4.0/24'

@description('The address prefix for the private links integration subnet')
param privateLinkSubnetPrefix string = '10.0.5.0/24'

var abbrs = loadJsonContent('../abbreviations.json')

var location = resourceGroup().location

param resourceToken string


resource gatewayPip 'Microsoft.Network/publicIPAddresses@2022-09-01' = {
  name: '${abbrs.networkPublicIPAddresses}${resourceToken}'
  location: location
  sku: {
    name: 'Standard'    
  }
  properties: {
    publicIPAllocationMethod: 'Static'
  }
}

// Virtual Network
resource virtualNetwork 'Microsoft.Network/virtualNetworks@2022-09-01' = {
  name: '${abbrs.networkVirtualNetworks}${resourceToken}'
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetAddressPrefix
      ]
    }
    subnets: [
      {
        name: '${abbrs.networkVirtualNetworksSubnets}default'
        properties: {
          addressPrefix: subnetAddressPrefix
        }
      }
      {
        name: '${abbrs.networkVirtualNetworksSubnets}aca'
        properties: {
          addressPrefix: acaSubnetPrefix
          privateEndpointNetworkPolicies: 'Enabled'
          delegations: [
            {             
              name: 'delegation'
              properties: {
                serviceName: 'Microsoft.App/environments'
              }
            }            
          ]          
        }
      }
      {
        name: '${abbrs.networkVirtualNetworksSubnets}gateway'
        properties: {
          addressPrefix: gatewaySubnetPrefix
        }
      }
      {
        name: '${abbrs.networkVirtualNetworksSubnets}private-link'
        properties: {
          addressPrefix: privateLinkSubnetPrefix
        }
      }
    ]
  }
}

// Output the subnet ID
output defaultSubnetId string = virtualNetwork.properties.subnets[0].id
output subnetId string = virtualNetwork.properties.subnets[1].id
output gatewaySubnetId string = virtualNetwork.properties.subnets[2].id
output privateLinkSubnetId string = virtualNetwork.properties.subnets[3].id
output gatewayPip string = gatewayPip.id
output virtualNetworkId string = virtualNetwork.id
