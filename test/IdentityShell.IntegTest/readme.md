# IdentityShell.IntegTest

This project illustrates how to write an integration test by hosting the IdentityShell with AspNets TestServer. This is most convenient if you want to write test cases which actually go through authetication and authorization scenarios without faking most of the infrastruture.
The hosted server implement alls the bells and whistles of the standalone server but uses in-memory databases instead of files so you didn't have to clean up behind it. 
Every test run git its own fresh instance. 

If run in the TesServer the IdentityShell will not open a console window or start a powershell. The configuration has to be created similar to an entity framework persietnce project by interaction with the DbContexts of each persistence instead.

