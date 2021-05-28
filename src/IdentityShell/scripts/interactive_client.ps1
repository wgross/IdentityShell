. $PSScriptRoot/utilities.ps1

clean_configurationstore

# scopes
$api1 = Set-IdentityApiScope -Name "api1"
$openid = Set-IdentityApiScope -Name "openid"
$profile = Set-IdentityApiScope -Name "profile"

# clients
$secret = New-IdentitySecret -PlainText "secret"
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

$alice = Set-TestUser @alice

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

$bob = Set-TestUser @bob
