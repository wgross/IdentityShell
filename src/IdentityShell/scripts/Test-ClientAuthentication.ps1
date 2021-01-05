Import-Module Pester

. $PSScriptRoot/common.ps1

Describe "Protecting an API using client credentials" {
    
    BeforeAll {
        clean_configurationstore
        clean_aspidentitystore
        $apiScope = Set-IdentityApiScope -Name api1 
        $apiResource = Set-IdentityApiResource -Name api1 -DisplayName "My Api" -Scopes $apiScope.Name
        $secretHash = "secret"|sha256base64
        $clientSecret = New-IdentitySecret -Value $secretHash    
        $client = Set-IdentityClient -ClientId client -AllowedGrantTypes client_credentials -ClientSecrets $clientSecret -AllowedScopes "api1","openid"
    }

    Context "Requesting client access token" {
        It "retrieves an access token from IdentityServer" {
            $discoveryDoc = Invoke-IdentityDiscoveryEndpoint                     
            $token = Invoke-IdentityTokenEndpoint -ClientId "client" -ClientSecret "secret" -Scopes "api1" -Endpoint $discoveryDoc.TokenEndpoint -TokenVariableName "tokenvar"
            $token | Should -Not -Be $null
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

    Context "Requesting user info doesn not work for client authentication" {
        It "IdentityServer provides a user info data" {       
            $discoveryDoc = Invoke-IdentityDiscoveryEndpoint 
            $token = Invoke-IdentityTokenEndpoint -ClientId "client" -ClientSecret "secret" -Scopes "api1" -Endpoint $discoveryDoc.TokenEndpoint
            $userInfo = Invoke-IdentityUserInfoEndpoint -Endpoint $discoveryDoc.UserInfoEndpoint -Token $token.AccessToken
            $userInfo | Should -Not -Be $null
            $userInfo.IsError|Should -Be $true            
        }
    }
}