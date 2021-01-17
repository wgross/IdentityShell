# IdentityShell.Cmdlets

Implements the Powershell Cmdlets which are injected to the Powershell instance running in the console windows of the IdentityShell. 

For each of the three persistence there are cmdlets to update or create an entity (Set-\*) of to read (Get-\*) or remove it from the storage (Remove-\*).

## Configuration Store

To inspect and edit the configuration store of IdentityServer4 the following cmdlets are provider:

```
Get-IdentityApiResource
Get-IdentityApiScope
Get-IdentityClient
Get-IdentityResource

Set-IdentityApiResource
Set-IdentityApiScope
Set-IdentityClient
Set-IdentityResource

Remove-IdentityApiResource
Remove-IdentityApiScope
Remove-IdentityClient
Remove-IdentityResource
```

Some configuration artefacts are not stored by itself but as part are given to one of the entities managned abopv as an argument:

``` 
New-IdentityClientClaim
New-IdentityScope
New-IdentitySecret
```

## Operational Store

Artefacts of teh Operations Stire can't be create ineractvely but browsed and removed:
```
Get-IdentityDeviceCode
Get-IdentityPersistedGrant

Remove-IdentityDeviceCode
Remove-IdentityPersistedGrant
```

## AspNetIdentity

AspNetIdentity entites ma be created edited or removed with:

```
Get-AspNetIdentityUser
Get-AspNetIdentityUserClaim

Set-AspNetIdentityUser
Set-AspNetIdentityUserClaim

Remove-AspNetIdentityUser
Remove-AspNetIdentityUserClaim
```

## Invoke IdentityServer4 Endpoints

Four cmdlets are provides using the IdentityServer4 client to call the its endpoints for Discovery, Token, TokenIntrospection and UserInfo:
```
Invoke-IdentityDiscoveryEndpoint
Invoke-IdentityTokenEndpoint
Invoke-IdentityTokenIntrospectionEndpoint
Invoke-IdentityUserInfoEndpoint
```

## Switching configuration

The cmdlet 
```
Restart-IdentityServer -ConfigurationStore .. -OperationalStore .. -UserStore ..
```

allows you to restart the hosted IdentityServer with alternative Sqlite-Db files pathes to switch between existing configurations easily. 



