The following documentation mirrors the IdentityServer4 [Quickstart documentation](https://docs.identityserver.io/en/latest/quickstarts/0_overview.html). It shows how the same configuration can be dine using the interactive IdentityShells cmdlets.

# [Protecting an API using Client Credentials](https://identityserver4.readthedocs.io/en/latest/quickstarts/1_client_credentials.html)

## [Defining an API Resource](https://docs.identityserver.io/en/latest/quickstarts/1_client_credentials.html#defining-an-api-resource)

The powershell cmdlets below add an api default scope. But since every API has to have at least scope 
you must add one here to make the API accesible.

```powershell
PS> Set-IdentityApiResource -Name api1 -DisplayName "My Api" -Scopes (New-IdentityScope -Name api1)

ApiSecrets  : {}
Scopes      : {api1}
Enabled     : True
Name        : api1
DisplayName : My Api
Description :
UserClaims  : {}
Properties  : {}
```
To inspect the API resources letering you may also us the Get-IdentityApiResource cmdlet:
```powershell
PS> Get-IdentityApiResource

ApiSecrets  : {}
Scopes      : {api1}
Enabled     : True
Name        : api1
DisplayName : My Api
Description : 
UserClaims  : {}
Properties  : {}
```

## [Defining the Client](https://docs.identityserver.io/en/latest/quickstarts/1_client_credentials.html#defining-the-client)

The identity server tutorial uses the word "secret" hashed as SHA-256. To repeat this in powershell copy&paste the filter function below to your console.

```powershell
filter sha256base64 {
    $bytes = [System.Text.Encoding]::UTF8.getBytes($_)
    $hash = [System.Security.Cryptography.HashAlgorithm]::Create("SHA256").ComputeHash($bytes)
    [System.Convert]::ToBase64String($hash)
}
```
Now we can create the ClientSecret and the Client habving a properly hashed secret:
```powershell
PS> $secrethash = "secret"|sha256base64
PS> Set-IdentityClient -ClientId client -AllowedGrantTypes ClientCredentials -ClientSecrets (New-IdentitySecret -Value $secrethash) -AllowedScopes "api1"

Enabled                           : True
ClientId                          : client
ProtocolType                      : oidc
ClientSecrets                     : {IdentityServer4.Models.Secret}
RequireClientSecret               : True
ClientName                        : 
Description                       : 
ClientUri                         :
LogoUri                           :
RequireConsent                    : True
AllowRememberConsent              : True
AllowedGrantTypes                 : {client_credentials}
RequirePkce                       : False
AllowPlainTextPkce                : False
AllowAccessTokensViaBrowser       : False
RedirectUris                      : {}
PostLogoutRedirectUris            : {}
FrontChannelLogoutUri             :
FrontChannelLogoutSessionRequired : True
BackChannelLogoutUri              :
BackChannelLogoutSessionRequired  : True
AllowOfflineAccess                : False
AllowedScopes                     : {api1}
AlwaysIncludeUserClaimsInIdToken  : False
IdentityTokenLifetime             : 300
AccessTokenLifetime               : 3600
AuthorizationCodeLifetime         : 300
AbsoluteRefreshTokenLifetime      : 2592000
SlidingRefreshTokenLifetime       : 1296000
ConsentLifetime                   :
RefreshTokenUsage                 : OneTimeOnly
UpdateAccessTokenClaimsOnRefresh  : False
RefreshTokenExpiration            : Absolute
AccessTokenType                   : Jwt
EnableLocalLogin                  : True
IdentityProviderRestrictions      : {}
IncludeJwtId                      : False
Claims                            : {}
AlwaysSendClientClaims            : False
ClientClaimsPrefix                : client_
PairWiseSubjectSalt               :
UserSsoLifetime                   :
UserCodeType                      :
DeviceCodeLifetime                : 300
AllowedCorsOrigins                : {}
Properties                        : {}
```
To inspect the clients lateron you may also use the cmdlet Get-IdentityClient:

```powershell
PS> Get-IdentityClient

Enabled                           : True
ClientId                          : client
ProtocolType                      : oidc
ClientSecrets                     : {IdentityServer4.Models.Secret}
RequireClientSecret               : True
ClientName                        : 
Description                       : 
ClientUri                         : 
LogoUri                           : 
RequireConsent                    : True
AllowRememberConsent              : True
AllowedGrantTypes                 : {client_credentials}
RequirePkce                       : False
AllowPlainTextPkce                : False
AllowAccessTokensViaBrowser       : False
RedirectUris                      : {}
PostLogoutRedirectUris            : {}
FrontChannelLogoutUri             : 
FrontChannelLogoutSessionRequired : True
BackChannelLogoutUri              : 
BackChannelLogoutSessionRequired  : True
AllowOfflineAccess                : False
AllowedScopes                     : {api1}
AlwaysIncludeUserClaimsInIdToken  : False
IdentityTokenLifetime             : 300
AccessTokenLifetime               : 3600
AuthorizationCodeLifetime         : 300
AbsoluteRefreshTokenLifetime      : 2592000
SlidingRefreshTokenLifetime       : 1296000
ConsentLifetime                   : 
RefreshTokenUsage                 : OneTimeOnly
UpdateAccessTokenClaimsOnRefresh  : False
RefreshTokenExpiration            : Absolute
AccessTokenType                   : Jwt
EnableLocalLogin                  : True
IdentityProviderRestrictions      : {}
IncludeJwtId                      : False
Claims                            : {}
AlwaysSendClientClaims            : False
ClientClaimsPrefix                : client_
PairWiseSubjectSalt               : 
UserSsoLifetime                   : 
UserCodeType                      : 
DeviceCodeLifetime                : 300
AllowedCorsOrigins                : {}
Properties                        : {}
```
The protocol flow for client credential authentication is documented in "examples/example-client-credentials.rest". Use this file with a [Visual Studio Code REST client extension](see: https://marketplace.visualstudio.com/items?itemName=humao.rest-client).

# [Interactive Applications](https://docs.identityserver.io/en/latest/quickstarts/2_interactive_aspnetcore.html)

## [Adding support for OpenID Connect Identity Scopes](https://docs.identityserver.io/en/latest/quickstarts/2_interactive_aspnetcore.html#adding-support-for-openid-connect-identity-scopes)

To go through this example scenario we extend the api configuration above with identity resources. 
IdentityServer4.Models contains already some predefined OpenId resources which we now add to the configuration store. IdentiyShell can acess these classes $directly:


```powershell
[IdentityServer4.Models.IdentityResources+OpenId]::new() | Set-IdentityResource 
[IdentityServer4.Models.IdentityResources+Profile]::new() | Set-IdentityResource
``` 
To inspect the added identity resources enter Get-IdentityResoure:
```powershell
PS> Get-IdentityResource

Required                : True
Emphasize               : False
ShowInDiscoveryDocument : True
Enabled                 : True
Name                    : openid
DisplayName             : Your user identifier
Description             : 
UserClaims              : {sub}
Properties              : {}

Required                : False
Emphasize               : True
ShowInDiscoveryDocument : True
Enabled                 : True
Name                    : profile
DisplayName             : User profile
Description             : Your user profile information (first name, last name, etc.)
UserClaims              : {name, family_name, given_name, middle_name...}
Properties              : {}
```

## [Adding Test Users](https://docs.identityserver.io/en/latest/quickstarts/2_interactive_aspnetcore.html#adding-test-users)

## Enabling Device Flow 

To enable devioce flow at the previously created client i'm adding the device flow grant type to the list of allowed grant types. The Set-IdentiyClient cmdlet, may be used to replace the grant type list with a new one:

```powershell
PS> Set-IdentityClient -ClientId "client" -AllowedGrantTypes client_credentials,urn:ietf:params:oauth:grant-type:device_code

...
AllowedGrantTypes                 : {client_credentials, urn:ietf:params:oauth:grant-type:device_code}
...
```
Now the endpoints of the identity server can be called as described in the example 'example-device-authorization.rest'. The result wil contain the device code and the user code of the issued token.

IdentityShell provide cmdlets to inspect the operational store. Like the client secret the user code and te device code are not stored in clear text. The are again hash with SHA-256. Utilize the function sha256base64 fro aboev ahaint to mke a usable value
```powershell
Get-IdentityDeviceCode -UserCode (535923422|sha256base64)

UserCode         : kmf1tjMQmNpetFYfALndcISmwEAu2MdPM5rkmbi11JQ=
CreationTime     : 26.06.2020 11:59:46
Lifetime         : 300
ClientId         : client
IsOpenId         : False
IsAuthorized     : False
RequestedScopes  : {api1}
AuthorizedScopes :
Subject          :
```
or alternatively:
```powershell
Get-IdentityDeviceCode -DeviceCode ("P2T4PguiCqRys9Yq4cv8UxTCUCltgwWh2IbaNbSKlh4"|sha256base64)

DeviceCode       : JgsO8+onUcfTzdJzXt0FDm7L1asCL0vm1O8/EM1bYNo=
CreationTime     : 26.06.2020 11:59:46
Lifetime         : 300
ClientId         : client
IsOpenId         : False
IsAuthorized     : False
RequestedScopes  : {api1}
AuthorizedScopes :
Subject          :
```


