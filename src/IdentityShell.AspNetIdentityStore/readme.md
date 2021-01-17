# IdentityShell.AspIdentityStore

Contains the migrations and the user class of the AspIdentity package for IdentityServer4 as you can download it from Nuget.

The migration can be re-created in command line with:

```
dotnet ef migrations add CreateIdentitySchema -c ApplicationDbContext -o Data/Migrations -s ..\IdentityShell\IdentityShell.csproj
```





