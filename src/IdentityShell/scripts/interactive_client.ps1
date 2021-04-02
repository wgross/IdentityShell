function clean_configurationstore {
    Get-IdentityApiScope|Remove-IdentityApiScope
    Get-IdentityApiResource|Remove-IdentityApiResource
    Get-IdentityResource|Remove-IdentityResource
    Get-IdentityClient|Remove-IdentityClient
    Get-TestUser|Remove-TestUser
}
filter sha256base64 {
    $bytes = [System.Text.Encoding]::UTF8.getBytes($_)
    $hash = [System.Security.Cryptography.HashAlgorithm]::Create("SHA256").ComputeHash($bytes)
    [System.Convert]::ToBase64String($hash)
}

clean_configurationstore

# scopes
$api1 = Set-IdentityApiScope -Name "api1"
$openid = Set-IdentityApiScope -Name "openid"
$profile = Set-IdentityApiScope -Name "profile"

# clients
$secret = New-IdentitySecret -Value ("secret"|sha256base64)
$clientApi = Set-IdentityClient -ClientId "client" -AllowedScopes @($api1.Name,$openid.Name,$profile.Name) -ClientSecrets $secret -AllowedGrantTypes client_credentials

$mvcClient = @{
    ClientId = "mvc" 
    ClientSecrets = $secret 
    AllowedGrantTypes = "authorization_code"
    RedirectUris = "https://localhost:5002/signin-oidc" 
    PostLogoutRedirectUris = "https://localhost:5002/signout-callback-oidc" 
    AllowedScopes = @("openid","profile")
}
$mvcClient = Set-IdentityClient @mvcClient

# users

$alice = @{
    SubjectId = "818727"
    Username = "alice"
    Password = "alice"
    Claims = @(
        New-Claim -Type "name" -Value "Alice Smith"
        New-Claim -Type "given_name" -Value "Alice"
        New-Claim -Type "family_name" -Value "Smith"
        New-Claim -Type "email" -Value "AliceSmith@email.com"
        New-Claim -Type "email_verified" -Value "true"
        New-Claim -Type "web_site" -Value "http://alice.com"
        New-Claim -Type "address" -ValueType "json" -Value (@{
            street_address = "One Hacker Way"
            locality = "Heidelberg"
            postal_code = 69118
            country = "Germany"
        } | ConvertTo-Json)
    )                      
}

Set-TestUser @alice

$bob = @{
    SubjectId = "88421113"
    Username = "bob"
    Password = "bob"
    Claims = @(
        New-Claim -Type "name" -Value "Bob Smith"
        New-Claim -Type "given_name" -Value "Bob"
        New-Claim -Type "family_name" -Value "Smith"
        New-Claim -Type "email" -Value "BobSmith@email.com"
        New-Claim -Type "email_verified" -Value "true"
        New-Claim -Type "web_site" -Value "http://bob.com"
        New-Claim -Type "address" -ValueType "json" -Value (@{
            street_address = "One Hacker Way"
            locality = "Heidelberg"
            postal_code = 69118
            country = "Germany"
        } | ConvertTo-Json)
    )                      
}

Set-TestUser @bob
