# IdentityShell.IdentityStore

This project defines the stores for IdentityServer4-configuration and -operation as it can be downloaded as Nuget package. 
The migrations can be created with:

```
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb -s ..\IdentityShell\IdentityShell.csproj

dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb -s ..\IdentityShell\IdentityShell.csproj
```

