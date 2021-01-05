# IdentityShell

..hosts an IdentityServer4 and a PowerShell in the same console process to allow interactive modification of the identity servers config.
The project is meant to be a playground for authentication/authorization configuration.

On Startup a powershell console opens which has cmdlets configured to manipulate the configuration, operation and identity stores. The endpints opf the identitysrver are reachable at port 5000 by default. The cmdlets also allow to restart the hosted IdentityServer4 endpoints without quitting the process. The executable can be started with the parameter '/noconsole' to behave like a normal net core service.

The configuration is made persistent using the Entity Framework based persistence for operation and configuration data provided for IdentityServer4 and Microsofts AspNetIdentity persistence using Sqlite.

## Building

Just clone the repository and 'dotnet build' the solution in the projects root should do the trick. 

## Usage examples
[here](docs/example.md)
