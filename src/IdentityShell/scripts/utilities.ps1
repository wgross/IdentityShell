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
