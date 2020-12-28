Import-Module Pester

. $PSScriptRoot/common.ps1

Describe "Protecting an API using resource owner flow" {
    
    Context "Configuration" {
        BeforeAll {
            clean_configurationstore
            clean_aspidentitystore
        }

        $scope = Set-IdentityApiScope -Name api1 
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
    }

    Context "Creating the OpenId identity resource representing the 'sub' claim" {
        $openid = [IdentityServer4.Models.IdentityResources+OpenId]::new() | Set-IdentityResource 
        It "Define the openid identity resource with claim 'sub'" {
            $openid|Should -Not -Be $null
            $openid.Name | Should -Be "openid"
            $openid.UserClaims | Should -Be "sub"
        }
    }
     
    Context "Creating the Profile indentity resource representing the user profile claims" {
        $profile = [IdentityServer4.Models.IdentityResources+Profile]::new() | Set-IdentityResource
        It "define the identity resource 'profile' with the predefined claims"  {
            $profile.UserClaims|Should -Be @("name","family_name","given_name","middle_name","nickname","preferred_username","profile","picture","website","gender","birthdate","zoneinfo","locale","updated_at")
        }
    }

    Context "Configuration of the client acessing the API" { 

        $secretHash = "secret"|sha256base64
        $clientSecret = New-IdentitySecret -Value $secretHash
        It "create a secret to be added to the client" {
            $clientSecret.Value|Should -Be $secretHash
        }
        
        $client = Set-IdentityClient -ClientId client -AllowedGrantTypes client_credentials,password -ClientSecrets $clientSecret -AllowedScopes "api1","openid","profile"
        It "Client has the secret $secretHash" {
            $client.ClientSecrets.Length|Should -Be 1
            $client.ClientSecrets[0].Value|Should -Be $secretHash
        }
        It "Client has access to the proctected api (api1) and the scopes openid and profile" {
            $client.AllowedScopes | Should -Be @("api1","openid","profile")
        }
    }

    Context "Defining the resource owner user" {

        $resourceOwner = Set-AspNetIdentityUser -UserName alice -NewPassword "Pass123$"
        It "Create user alice with password" {
            $resourceOwner.UserName|Should -Be "alice"
            $resourceOwner.PasswordHash|Should -Not -Be $null
        }

        $aliceClaims = @(
            New-Claim -Type name -Value "Alice Smith"
            New-Claim -Type given_name -Value "Alice"
            New-Claim -Type family_name -Value "Smith"
            New-Claim -Type email -Value "AliceSmith@email.com"
            New-Claim -Type email_verified -Value true -ValueType Boolean
            New-Claim -Type website -Value "http://alice.com"
        )
        $aliceClaims|Set-AspNetIdentityUserClaim -UserName "alice"
    }

    Context "Request token for ap1" {

        $discoveryDoc = Invoke-IdentityDiscoveryEndpoint 
        $token = Invoke-IdentityTokenEndpoint -ClientId "client" -ClientSecret "secret" -Scopes "api1" -UserName "alice" -Password "Pass123$" -Endpoint $discoveryDoc.TokenEndpoint -TokenVariableName "tokenvar"
        It "was returned" {
            $token | Should -Not -Be $null
        }
        It "was stored in a tolen variable" {
            $tokenvar|Should -Be $token.AccessToken
        }
    }

    Context "Alices UserInfo" {
        
        $discoveryDoc = Invoke-IdentityDiscoveryEndpoint 
        $token = Invoke-IdentityTokenEndpoint -ClientId "client" -ClientSecret "secret" -Scopes @("api1","openid","profile") -UserName "alice" -Password "Pass123$" -Endpoint $discoveryDoc.TokenEndpoint
        $userInfo = Invoke-IdentityUserInfoEndpoint -Endpoint $discoveryDoc.UserInfoEndpoint -Token $token.AccessToken
        It "User info could be retrieved" {
            $userInfo | Should -Not -Be $null
        }

        filter claim_value ($type) { $_.Claims.Where({$_.Type -eq $type}).Value }

        $alice = Get-AspNetIdentityUser -UserName alice
        It "contains the sub claims" {
            $userInfo.Claims.Where({$_.Type -eq "sub"}).Value | Should -Be $alice.Id
            $userInfo|claim_value -type "sub" | Should -Be $alice.Id
        }
        It "contains the profile claims" {
            $userInfo|claim_value -type "name" | Should -Be "Alice Smith"
            $userInfo|claim_value -type "given_name" | Should -Be "Alice"
            $userInfo|claim_value -type "family_name" | Should -Be "Smith"
            $userInfo|claim_value -type "website" | Should -Be "http://alice.com"
            $userInfo|claim_value -type "preferred_username" | Should -Be "alice"
        }
        It "doesn't contain non-profile claims" {
            $userInfo|claim_value -type "email" | Should -Be $null
            $userInfo|claim_value -type "email_verified" | Should -Be $null
        }
    }
}
        