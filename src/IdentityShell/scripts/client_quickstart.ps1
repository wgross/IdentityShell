. $PSScriptRoot/utilities.ps1

clean_configurationstore

$secret = New-IdentitySecret -PlainText "secret"
$api1 = Set-IdentityApiScope -Name "api1"
$client = Set-IdentityClient -ClientId "client" -AllowedScopes $api1.Name -ClientSecrets $secret -AllowedGrantTypes client_credentials
