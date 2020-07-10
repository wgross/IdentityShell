The following documentation follows the IdentityServer4 [Quickstart documentation](https://docs.identityserver.io/en/latest/quickstarts/0_overview.html) through several authentication scenarios. It shows how the necessary configuration can be done with the IdentityShells cmdlets. It also provides HTTP requests examples usable with VS Codes Rest client to interact with the endpoints of the identity server.

# [Protecting an API using Client Credentials](https://identityserver4.readthedocs.io/en/latest/quickstarts/1_client_credentials.html)

This protocol flow is used to grant access to a client software identified by a unique client id and a client secret to api resources.

## [Defining an API Resource](https://docs.identityserver.io/en/latest/quickstarts/1_client_credentials.html#defining-an-api-resource)

The powershell cmdlets below add an api default scope. Every API has to have at least one scope:

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
To inspect the API resources use the Get-IdentityApiResource cmdlet:
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

## (Defining the Client)[https://docs.identityserver.io/en/latest/quickstarts/1_client_credentials.html#defining-the-client]

The identity server tutorial uses the word "secret" hashed as SHA-256. For hashing the secret in powershell copy&paste the filter function below to your console.

```powershell
filter sha256base64 {
    $bytes = [System.Text.Encoding]::UTF8.getBytes($_)
    $hash = [System.Security.Cryptography.HashAlgorithm]::Create("SHA256").ComputeHash($bytes)
    [System.Convert]::ToBase64String($hash)
}
```
Now the client can be created with a properly hashed client secret:
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
To inspect the client use the cmdlet Get-IdentityClient:
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
The protocol flow for client credential authentication is shown in [example-client-credentials.rest](examples/example-client-credentials.rest). Use this file with a [Visual Studio Code REST client extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client).

# [Protecting an API using Passwords an Resource Owner Flow](http://docs.identityserver.io/en/stable/quickstarts/2_resource_owner_passwords.html#)

This scenario grants access to a interative use at a client software uncapable of using a web page as login dialog like a fat client or example.
The users are stored persistently in an identity store defined by the 
[provided nuget package](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity/).
We assume the client configuration from above. The lines below set the allowed grants including resource owner flow:
```powershell
PS> $client=Get-IdentityClient 
PS> $client|Set-IdentityClient -AllowedGrantTypes client_credentials,ResourceOwnerPassword,DeviceFlow
```
 Now a user is required from the AspNetIdentity example:
```powershell
PS> Set-AspNetIdentityUser -UserName alice -NewPassword "Pass123$"
```
To inspect the users use cmdlet Get-AspNetIdentityUser:
```powershell
PS> Get-AspNetIdentityUser

Id                   : acf41cc1-e964-4569-b3a2-79cce4d99bcf
UserName             : alice
NormalizedUserName   : ALICE
Email                :
NormalizedEmail      :
EmailConfirmed       : False
PasswordHash         : AQAAAAEAACcQAAAAEAP0awBMegucMtMX0e3YSvQ5SX8f+nYzuT1Zb8nUrbx0bfoUZxhcEy5TUJYzVFDVeQ==
SecurityStamp        : VHBZUWN4WMXKMROQLA5HV7TXRBLC7OGH
ConcurrencyStamp     : 4c175ef9-21fd-455f-a2fc-823df60186a8
PhoneNumber          :
PhoneNumberConfirmed : False
TwoFactorEnabled     : False
LockoutEnd           :
LockoutEnabled       : True
AccessFailedCount    : 0
```
Setting the users properties doesn't implicitely create claims for the user of the same semantic. This must be done independently.
If you create these claims you will see that the arguments 'Type' and 'ValueType' provide argument completion for known values.
```powershell
PS> $claims = @(
        New-Claim -Type Name -Value "Alice Smith"
        New-Claim -Type GivenName -Value "Alice"
        New-Claim -Type FamilyName -Value "Smith"
        New-Claim -Type Email -Value "AliceSmith@email.com"
        New-Claim -Type EmailVerified -Value true -ValueType Boolean
        New-Claim -Type WebSite -Value "http://alice.com"
    )

PS> $claims|Set-AspNetIdentityUserClaim -UserName alice 
```
To check the claims invoke the cmdlet Get-AspNetIdentityUserClaim:
```powershell
PS> Get-AspNetIdentityUserClaim -UserName alice
```
The configuraton is now finsihed. Please refer to [example-resource-owner.rest](examples/example-resource-owner.rest) for the HTTP messages the client sends during this protocol flow.

## Getting information about the logged in User

OpenId defines an endoint to retrieve information about the logged in user. To prepare this teh default open id scopes 'openid' and 'profile' 
have to present in as an identity resource. Thes ecan be creates directly from the identit serves model:
```powershell
PS> [IdentityServer4.Models.IdentityResources+OpenId]::new() | Set-IdentityResource 
PS> [IdentityServer4.Models.IdentityResources+Profile]::new() | Set-IdentityResource
``` 
To inspect the added identity resources use cmdlet Get-IdentityResoure:
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
Add the scopes names to the allowed scopes of the client:
```powershell
PS> Set-IdentityClient -ClientId client -AllowedScopes api1,openid,profile
```
If the token-request asks for access to the scopes "openid" and "profile" the user-info endpoint will return all claims defined in identity resources openid and profile. The extended respource owner example in [example-resource-owner-userinfo.rest](examples/example-resource-owner-userinfo.rest) shows HTTP requests.

## Enabling Device Flow 

To enable device flow at the previously created client the device flow grant type must be added to the list of allowed grant types:
```powershell
PS> Set-IdentityClient -ClientId "client" -AllowedGrantTypes client_credentials,urn:ietf:params:oauth:grant-type:device_code

...
AllowedGrantTypes                 : {client_credentials, urn:ietf:params:oauth:grant-type:device_code}
...
```
Now the endpoints of the identity server can be called as shown in the example (example-device-authorization.rest)[examples/example-device-authorization.rest]. The result wil contain the device code and the user code of the issued token.

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


