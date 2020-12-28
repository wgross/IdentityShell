function clean_configurationstore {
    Get-IdentityApiScope|Remove-IdentityApiScope
    Get-IdentityApiResource|Remove-IdentityApiResource
    Get-IdentityResource|Remove-IdentityResource
    Get-IdentityClient|Remove-IdentityClient
}

function clean_aspidentitystore {
    Get-AspNetIdentityUser|ForEach-Object {
        Get-AspNetIdentityUserClaim -UserName $_.UserName|Remove-AspNetIdentityUserClaim -UserName $_.UserName
    }
    Get-AspNetIdentityUser|Remove-AspNetIdentityUser
}

filter sha256base64 {
    $bytes = [System.Text.Encoding]::UTF8.getBytes($_)
    $hash = [System.Security.Cryptography.HashAlgorithm]::Create("SHA256").ComputeHash($bytes)
    [System.Convert]::ToBase64String($hash)
}