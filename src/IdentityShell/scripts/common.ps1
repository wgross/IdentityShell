function clean_configurationstore {
    Get-IdentityApiScope|Remove-identityApiScope
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