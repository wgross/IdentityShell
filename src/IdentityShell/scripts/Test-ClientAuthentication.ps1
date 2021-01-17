Import-Module Pester

. $PSScriptRoot/common.ps1

clean_configurationstore
clean_aspidentitystore

# define the protected assets
$apiScope = Set-IdentityApiScope -Name "api-access"
$apiResource = Set-IdentityApiResource -Name "http://my/api" -Scopes $apiScope.Name

# define the client allowed to access the assets
$clientSecret = New-IdentitySecret -Value ("secret"|sha256base64)
$client = Set-IdentityClient -ClientId "api-client" -AllowedGrantTypes "client_credentials" -ClientSecrets $clientSecret -AllowedScopes "api-access"

Describe "Protecting an API using client credentials" {

    Context "Requesting client access token" {
        It "retrieves an access token from IdentityServer" {
            $discoveryDoc = Invoke-IdentityDiscoveryEndpoint                     
            $token = Invoke-IdentityTokenEndpoint -ClientId "api-client" -ClientSecret "secret" -Scopes "api-access" -Endpoint $discoveryDoc.TokenEndpoint -TokenVariableName "tokenvar"
            $token | Should -Not -Be $null
            $tokenvar | Should -Be $token.AccessToken
        }
    }
}