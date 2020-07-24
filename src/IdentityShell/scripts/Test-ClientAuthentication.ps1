Import-Module Pester

filter sha256base64 {
    $bytes = [System.Text.Encoding]::UTF8.getBytes($_)
    $hash = [System.Security.Cryptography.HashAlgorithm]::Create("SHA256").ComputeHash($bytes)
    [System.Convert]::ToBase64String($hash)
}

Describe "Protecting an API using client credentials" {
    
    Context "Configuration" {
        BeforeAll {
            Get-IdentityApiScope|Remove-identityApiScope
            Get-IdentityApiResource|Remove-identityApiResource
            Get-IdentityClient|Remove-IdentityClient
        }
        
        $scope=Set-IdentityApiScope -Name api1 
        It "Create an api scope api1" {
            $scope|Should -Not -Be $null
            $scope.Name | Should -Be "api1"
        }

        $api = Set-IdentityApiResource -Name api1 -DisplayName "My Api" -Scopes "api1"
        It "Create the api resource Api1" {
            $api|Should -Not -Be $null
            $api.Name | Should -Be "api1"
            $api.Scopes | Should -Be "api1"
        }

        $secretHash = "secret"|sha256base64
        $clientSecret=New-IdentitySecret -Value $secretHash
        It "create a secret to be added to the client" {
            $clientSecret.Value|Should -Be $secretHash
        }

        $client = Set-IdentityClient -ClientId client -AllowedGrantTypes client_credentials -ClientSecrets $clientSecret -AllowedScopes "api1"
        It "Create a client having access to api1 using the secret from above" {
            $client.ClientSecrets.Length|Should -Be 1
            $client.ClientSecrets[0].Value|Should -Be $secretHash
            $client.AllowedScopes | Should -Be "api1"
        }
    }

    Context "Token Endpoint" {
        $discoveryDoc = Invoke-IdentityDiscoveryEndpoint 
        It "Request discovery document with token endpoint" {
            $discoveryDoc|Should -Not -Be $null
            $discoveryDoc.TokenEndpoint|Should -Not -Be $null
        }
    
        $token = Invoke-IdentityTokenEndpoint -ClientId "client" -ClientSecret "secret" -Scope "api1" -Endpoint $discoveryDoc.TokenEndpoint -TokenVariableName "tokenvar"
        It "Request token from authorization endpoint" {
            $token | Should -Not -Be $null
        }
        It "Token variable was set with AccessToken" {
            $tokenvar|Should -Be $token.AccessToken
        }
    }

    Context "User Info Endpoint" {
        $discoveryDoc = Invoke-IdentityDiscoveryEndpoint 
        $token = Invoke-IdentityTokenEndpoint -ClientId "client" -ClientSecret "secret" -Scope "api1" -Endpoint $discoveryDoc.TokenEndpoint

        It "Contains the user info data" {
            $userInfo = Invoke-IdentityUserInfoEndpoint -Endpoint $discoveryDoc.UserInfoEndpoint -Token $token.AccessToken
            $userInfo | Should -Not -Be $null
        }
    }
}