@description('The address prefix for the virtual network')
param vnetAddressPrefix string = '10.0.0.0/16'

@description('The address prefix for the subnet')
param subnetAddressPrefix string = '10.0.1.0/24'

param acaSubnetPrefix string = '10.0.2.0/23'

var abbrs = loadJsonContent('../abbreviations.json')

var location = resourceGroup().location

param resourceToken string

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
        }
      }
    ]
  }
}

// Output the subnet ID
output defaultSubnetId string = virtualNetwork.properties.subnets[0].id
output subnetId string = virtualNetwork.properties.subnets[1].id
