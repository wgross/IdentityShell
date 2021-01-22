# IdentityShell

is a experimentation/playground for OpenId authorization based on [IdentityServer4](https://github.com/IdentityServer/IdentityServer4). 

Technically it is a net5 console app hosting an IdentityServer4 and displaying a powershell console to the user. The powershell provides commandlets to manage the configuration and operational store of the Identity server and the user store which is based on [Microsofts AspNetCore Identity framework](https://github.com/dotnet/aspnetcore/tree/main/src/Identity).

The stores use Sqlite as persistence.

More informnation ono the  on the powershell commandlets provided can be found here : [src/IdentityShell.Cmdlets](https://github.com/wgross/IdentityShell/tree/main/src/IdentityShell.Cmdlets)

Usage Examples can be found in [docs/example.md](https://github.com/wgross/IdentityShell/blob/main/docs/example.md).

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

## Usage examples
[here](docs/example.md)
