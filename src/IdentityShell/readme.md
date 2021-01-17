# IdentityShell

The main project of this repo. Just start the project with dotnet run or in the debugger. 
By default the databases for the three stores are created in the root of the project as:

```
PS> dir .\src\IdentityShell\*.db | select Name

Name
----
IdentityShell.ConfigurationStore.db
IdentityShell.OperationalStore.db
IdentityShell.UserStore.db
```

By default an powershell console is started by the project. It ios psosible to deisable that by running the service with the parameter '/noconsole':

```
PS> dotnet run /noconsole
```

IdentityShell will still occupy the console window but only for logging output.


## IdentityShell/scripts

This folder contains powershell example scripts to configure and verify some authorization scenarios.

common.ps1 provides handy functions which are sourced by all the scenario scripts like cleaning the current database and hashing a secrets with SHA256.

## IdentityShell/Views and IdentityShell/Quickstart

Plain vanilla IdentityServer4 Quickstart UI as it can be downloaded from th IdentityServer4 github repo. 
Use the updateUI.ps1 script to fetch a newer version.

## IdentityServer4 as Submodule 

IdentityShell references the IdentityServer4 core part as a submodule instead of a Nuget package. This helps greatly with trouble shooting of authorization issues. 
It is possible to step through the validation of any request to find the missing parts in your configuration scenario. 



