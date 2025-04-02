@description('The address prefix for the virtual network')
param vnetAddressPrefix string = '10.0.0.0/16'

@description('The address prefix for the subnet')
param subnetAddressPrefix string = '10.0.1.0/24'

// Virtual Network
resource virtualNetwork 'Microsoft.Network/virtualNetworks@2022-09-01' = {
  name: '${abbrs.virtualNetworks}${resourceToken}'
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetAddressPrefix
      ]
    }
    subnets: [
      {
        name: '${abbrs.subnets}${resourceToken}'
        properties: {
          addressPrefix: subnetAddressPrefix
        }
      }
    ]
  }
}

// Output the subnet ID
output subnetId string = virtualNetwork.properties.subnets[0].id
