
function clean_configurationstore {
    Get-IdentityApiScope|Remove-IdentityApiScope
    Get-IdentityApiResource|Remove-IdentityApiResource
    Get-IdentityResource|Remove-IdentityResource
    Get-IdentityClient|Remove-IdentityClient
}
filter sha256base64 {
    $bytes = [System.Text.Encoding]::UTF8.getBytes($_)
    $hash = [System.Security.Cryptography.HashAlgorithm]::Create("SHA256").ComputeHash($bytes)
    [System.Convert]::ToBase64String($hash)
}

clean_configurationstore

$secret = New-IdentitySecret -Value ("secret"|sha256base64)
$api1 = Set-IdentityApiScope -Name "api1"
$client = Set-IdentityClient -ClientId "client" -AllowedScopes $api1.Name -ClientSecrets $secret -AllowedGrantTypes client_credentials
