# IdentityShell
A hosts an IdentityServer4 and a PowerShell in the same console process to allow interactive modification of the identity servers config.
The project is meant to be a playground for authentication/authorization configuration.

Also the exectable can be started with the parameter /noconsole to behave like a normal net core service.

The confguration is made persistent using the Entity Framework based persistence for operation and configuartion data provided for IdentityServer4 and Microsofts AspNetIdentity persistence.
On Startup a powershell console opens which has cmdlets configured to manipulate the configuration, operation and identity stores. It also allows to restart the hosted IdentityServer4 endpoints without quiting the process.
As persistence an embedded Sqlite database is used.

## Usage examples
[here](docs/example.md)
