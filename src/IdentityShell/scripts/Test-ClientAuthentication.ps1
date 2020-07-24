Import-Module Pester
. $PSScriptRoot/common.ps1

filter sha256base64 {
    $bytes = [System.Text.Encoding]::UTF8.getBytes($_)
    $hash = [System.Security.Cryptography.HashAlgorithm]::Create("SHA256").ComputeHash($bytes)
    [System.Convert]::ToBase64String($hash)
}

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
        $clientSecret=New-IdentitySecret -Value $secretHash
        It "has a secret added to the client" {
            $clientSecret.Value|Should -Be $secretHash
        }

        $client = Set-IdentityClient -ClientId client -AllowedGrantTypes client_credentials -ClientSecrets $clientSecret -AllowedScopes "api1","openid"
        It "rewqures a client with access to 'Ap1'" {
            $client.ClientSecrets.Length|Should -Be 1
            $client.ClientSecrets[0].Value|Should -Be $secretHash
            $client.AllowedScopes | Should -Be @("api1","openid")
        }
    }

    Context "Request client access token" {

        $discoveryDoc = Invoke-IdentityDiscoveryEndpoint         
        $token = Invoke-IdentityTokenEndpoint -ClientId "client" -ClientSecret "secret" -Scopes "api1" -Endpoint $discoveryDoc.TokenEndpoint -TokenVariableName "tokenvar"
        It "retrieved a token from IdentityServer" {
            $token | Should -Not -Be $null
        }
        It "set the given token varaible" {
            $tokenvar|Should -Be $token.AccessToken
        }
    }

    Context "Requesting user info" {

        $discoveryDoc = Invoke-IdentityDiscoveryEndpoint 
        $token = Invoke-IdentityTokenEndpoint -ClientId "client" -ClientSecret "secret" -Scopes "api1" -Endpoint $discoveryDoc.TokenEndpoint
        It "IdentityServer provides a user info data" {
            $userInfo = Invoke-IdentityUserInfoEndpoint -Endpoint $discoveryDoc.UserInfoEndpoint -Token $token.AccessToken
            $userInfo | Should -Not -Be $null
        }
    }
}