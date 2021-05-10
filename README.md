# IdentityShell

is a experimentation environment/playground for OpenId Connect authorization based on [IdentityServer5](https://duendesoftware.com/products/identityserver). 

Technically it is a .Net5 console app displaying a powershell console to the user and also hosting an IdentityServer5 in the background. 
The powershell provides commandlets to manage the configuration of the IdentityServer. 

Is also provides a [Quickstart UI](https://github.com/DuendeSoftware/IdentityServer.Quickstart.UI).

Configuration data is stored in memory only. (I've removed the Sqlite persistence with the upgrade to Identity Server V5 bacuse it diodn't contribute much to the goal of the project)

The powershell host is built with [Microsoft.Powershell.SDK](https://www.nuget.org/packages/Microsoft.PowerShell.SDK/) having the cmdlets implemented at from [IdentityShell/Commands](https://github.com/wgross/IdentityShell/tree/main/src/IdentityShell/Commands)

An simple example of a client authentication configuration in IdentityShell console looks like this:
```powershell
# define the protected assets
$apiScope = Set-IdentityApiScope -Name "api-access"
$apiResource = Set-IdentityApiResource -Name "http://my/api" -Scopes $apiScope.Name

# define the client allowed to access the assets
$clientSecret = New-IdentitySecret -Value ("secret"|sha256base64)
$client = Set-IdentityClient -ClientId "api-client" -AllowedGrantTypes "client_credentials" -ClientSecrets $clientSecret -AllowedScopes "api-access"
```
More  examples can be found in [IdentityShell/scripts](https://github.com/wgross/IdentityShell/tree/main/src/IdentityShell/scripts).

## Cloning the repository
IdentityServer5 itself isn't reference as a nuget package but as a submodule. This makes it easy to debug the inner workings of it if authorizatin problems occur.
```
git clone https://github.com/wgross/IdentityShell.git --recurse-submodules
```

## Building
```
cd .\IdentityShell
dotnet build
```

## Run the server
```
cd .\src\IdentityShell
dotnet run
```` 
The server creates empty databases on start by default at the projects root.
