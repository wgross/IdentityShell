# IdentityShell

is a experimentation environment/playground for OpenId authorization based on [IdentityServer4](https://github.com/IdentityServer/IdentityServer4). 

Technically it is a .Net5 console app hosting an IdentityServer4 and displaying a powershell console to the user. The powershell provides commandlets to manage the configuration and operational store of the IdentityServer and the membership store which is based on [Microsofts AspNetCore Identity framework](https://github.com/dotnet/aspnetcore/tree/main/src/Identity). 

The server is composed from the available vanilla extesions packages:
* [IdentityServer4.AspNetIdentity](https://www.nuget.org/packages/IdentityServer4.AspNetIdentity/)
* [IdentityServer4.EntityFramework](https://www.nuget.org/packages/IdentityServer4.EntityFramework/)
* [IdentityServer4.EntityFramework.Storage](https://www.nuget.org/packages/IdentityServer4.EntityFramework.Storage/)
* [IdentityServer4.Storage](https://www.nuget.org/packages/IdentityServer4.Storage/)

Is uses as well the [Quickstart UI](https://github.com/IdentityServer/IdentityServer4.Quickstart.UI).

The stores use Sqlite as a database.

The powershell host is built with [Microsoft.Powershell.SDK](https://www.nuget.org/packages/Microsoft.PowerShell.SDK/) having the cmdlets injected from [IdentityShell.Cmdlets](https://github.com/wgross/IdentityShell/blob/main/src/IdentityShell.Cmdlets/readme.md)

An simple example of a client authentication configuration in IdentityShell console looks like this:
```powershell
# define the protected assets
$apiScope = Set-IdentityApiScope -Name "api-access"
$apiResource = Set-IdentityApiResource -Name "http://my/api" -Scopes $apiScope.Name

# define the client allowed to access the assets
$clientSecret = New-IdentitySecret -Value ("secret"|sha256base64)
$client = Set-IdentityClient -ClientId "api-client" -AllowedGrantTypes "client_credentials" -ClientSecrets $clientSecret -AllowedScopes "api-access"
```
More  examples can be found in [docs/example.md](https://github.com/wgross/IdentityShell/blob/main/docs/example.md).

Also thereare examples for using Identityhell in integration tests with Microsofts [WebApplicationFactory](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-5.0) in [IdentitShell/tests/IndetitShell.IntegTests](https://github.com/wgross/IdentityShell/blob/main/test/IdentityShell.IntegTest/readme.md) and obiously the cmdlets of this project are also tested in [IdentityShell.Cmdlets.Tests](https://github.com/wgross/IdentityShell/tree/main/test/IdentityShell.Cmdlets.Test).

## Cloning the repository
IdentityServer4 itself isn't reference as a nuget package but as a submodule. This makes it easy to debug the inner workings of it if authorizatin problems occur.
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
