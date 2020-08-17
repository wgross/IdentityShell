Import-Module Pester

. $PSScriptRoot/common.ps1

Describe "Protecting an APSNET core Application" {

    BeforeAll {
        clean_configurationstore
        clean_aspidentitystore
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

    Context "Configuration of the client providing the UI" {  
        
        $secretHash = "secret"|sha256base64
        $clientSecret = New-IdentitySecret -Value $secretHash
        It "has a secret added to the client" {
            $clientSecret.Value|Should -Be $secretHash
        }

        $client = Set-IdentityClient -ClientId "mvc" -ClientSecrets $clientSecret -AllowedGrantTypes authorization_code  -AllowedScopes "openid","profile"`
            -RedirectUris "http://localhost:5001/signin-oidc" -PostLogoutRedirectUris "http://localhost:5001/signout-callback-oidc"
        It "requires a client with access to scopes 'profile' and 'openid'" {
            $client.ClientSecrets.Length|Should -Be 1
            $client.ClientSecrets[0].Value|Should -Be $secretHash
            $client.AllowedScopes | Should -Be @("openid","profile")
            $client.RedirectUris | Should -Be @("http://localhost:5001/signin-oidc")
            $client.PostLogoutRedirectUris | Should -Be @("http://localhost:5001/signout-callback-oidc")
        }
    }

    Context "Defining the interactive user" {

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

    Context "Start the aplication" {
        Start-Process -FilePath "dotnet" -WorkingDirectory "$PSScriptRoot\..\..\..\examples\interactiveUI" -ArgumentList @("run")
        Start-Process "http://localhost:5001"
    }
}

