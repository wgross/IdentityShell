Import-Module Pester

. $PSScriptRoot/common.ps1

clean_configurationstore
clean_aspidentitystore

# define the protected assets
$apiScope = Set-IdentityApiScope -Name "api-access" 
$apiResource = Set-IdentityApiResource -Name "http://my/api" -Scopes $apiScope.Name
$openIdIdentityResource = [IdentityServer4.Models.IdentityResources+OpenId]::new() | Set-IdentityResource 
$profileIdentityResource = [IdentityServer4.Models.IdentityResources+Profile]::new() | Set-IdentityResource

# define the client allowed to access the assets
$clientSecret = New-IdentitySecret -Value ("secret"|sha256base64)
$client = Set-IdentityClient -ClientId client -AllowedGrantTypes client_credentials,password -ClientSecrets $clientSecret -AllowedScopes "api-access","openid","profile"

# define the user allowed to access the assets using the allowed client
$resourceOwner = Set-AspNetIdentityUser -UserName alice -NewPassword "Pass123$"   
$aliceClaims = @(
    New-Claim -Type name -Value "Alice Smith"
    New-Claim -Type given_name -Value "Alice"
    New-Claim -Type family_name -Value "Smith"
    New-Claim -Type email -Value "AliceSmith@email.com"
    New-Claim -Type email_verified -Value true -ValueType Boolean
    New-Claim -Type website -Value "http://alice.com"
)

$aliceClaims|Set-AspNetIdentityUserClaim -UserName "alice"

Describe "protected an API using client Credentials and Resource Owner password Flow" {
    
    Context "Request access token for alice" {
        
        $discoveryDoc = Invoke-IdentityDiscoveryEndpoint 
        $token = Invoke-IdentityTokenEndpoint -ClientId "client" -ClientSecret "secret" -Scopes "api-access" -UserName "alice" -Password "Pass123$" -Endpoint $discoveryDoc.TokenEndpoint
    
        It "access token was returned" {
            $token | Should -Not -Be $null
        }
    }

    Context "Request user info for alice" {

        $discoveryDoc = Invoke-IdentityDiscoveryEndpoint 
        $token = Invoke-IdentityTokenEndpoint -ClientId "client" -ClientSecret "secret" -Scopes @("api-access","openid","profile") -UserName "alice" -Password "Pass123$" -Endpoint $discoveryDoc.TokenEndpoint
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
        