Import-Module Pester

. $PSScriptRoot/common.ps1

Describe "Protecting an API using client credentials" {
    
    BeforeAll {
        clean_configurationstore
        clean_aspidentitystore
    }

    Context "Configuration of the protected API" {
   
        $apiScope = Set-IdentityApiScope -Name api1 
        It "requires an api scope 'api1'" {
            $apiScope|Should -Not -Be $null
            $apiScope.Name | Should -Be "api1"
        }

        $apiResource = Set-IdentityApiResource -Name api1 -DisplayName "My Api" -Scopes $apiScope.Name
        It "defines an api resource" {
            $apiResource|Should -Not -Be $null
            $apiResource.Name | Should -Be "api1"
            $apiResource.Scopes | Should -Be "api1"
        }
    }

    Context "Configuration of the client acessing the API" {

        $secretHash = "secret"|sha256base64
        $clientSecret = New-IdentitySecret -Value $secretHash
        It "has a secret added to the client" {
            $clientSecret.Value|Should -Be $secretHash
        }

        $client = Set-IdentityClient -ClientId client -AllowedGrantTypes client_credentials -ClientSecrets $clientSecret -AllowedScopes "api1","openid"
        It "rewqures a client with access to 'Api1'" {
            $client.ClientSecrets.Length|Should -Be 1
            $client.ClientSecrets[0].Value|Should -Be $secretHash
            $client.AllowedScopes | Should -Be @("api1","openid")
        }
    }

    Context "Requesting client access token" {

        $discoveryDoc = Invoke-IdentityDiscoveryEndpoint         
        $token = Invoke-IdentityTokenEndpoint -ClientId "client" -ClientSecret "secret" -Scopes "api1" -Endpoint $discoveryDoc.TokenEndpoint -TokenVariableName "tokenvar"
        It "retrieves an access token from IdentityServer" {
            $token | Should -Not -Be $null
        }
        It "sets the given token varaible" {
            $tokenvar|Should -Be $token.AccessToken
        }
    }

    # Context "Requesting token metadata" {
    #     $discoveryDoc = Invoke-IdentityDiscoveryEndpoint         
    #     $token = Invoke-IdentityTokenEndpoint -ClientId "client" -ClientSecret "secret" -Scopes "api1" -Endpoint $discoveryDoc.TokenEndpoint
    #     $tokenIntrospection = Invoke-IdentityTokenIntrospectionEndpoint  -ApiResource "api1" -Endpoint $discoveryDoc.IntrospectionEndpoint -Token $token.AccessToken
    #     It "retrieves a token introspection information from IdentityServer" {
    #         $tokenIntrospection.IsError | Should -Be $false
    #         $tokenIntrospection | Should -Not -Be $null
    #     }
    # }

    Context "Requesting user info" {

        $discoveryDoc = Invoke-IdentityDiscoveryEndpoint 
        $token = Invoke-IdentityTokenEndpoint -ClientId "client" -ClientSecret "secret" -Scopes "api1" -Endpoint $discoveryDoc.TokenEndpoint
        $userInfo = Invoke-IdentityUserInfoEndpoint -Endpoint $discoveryDoc.UserInfoEndpoint -Token $token.AccessToken
        It "IdentityServer provides a user info data" {
            $userInfo.IsError|Should -Be $false
            $userInfo | Should -Not -Be $null
        }
    }
}