using IdentityModel.Client;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerAspNetIdentity.Models;
using IdentityShell.IntegTest.TestServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace IdentityShell.IntegTest
{
    public class IdentityServerConfigurationTest
    {
        public IdentityServerConfigurationTest()
        {
            //File.Copy(
            //    @"D:\src\IdentityShell\src\IdentityShell\bin\Debug\net5.0\IdentityShell.deps.json",
            //    @"D:\src\IdentityShell\test\IdentityShell.IntegTest\bin\Debug\net5.0\IdentityShell.deps.json", true);

            this.IdentityShell = new TestServerHostFactory();
        }

        public TestServerHostFactory IdentityShell { get; }

        [Fact]
        public async Task Configure_client_credentials_and_fetch_token()
        {
            // ARRANGE

            using var cfgDbx = this.IdentityShell.Services.GetRequiredService<ConfigurationDbContext>();

            cfgDbx.ApiScopes.Add(new ApiScope
            {
                Name = "api-access"
            });

            cfgDbx.ApiResources.Add(new ApiResource
            {
                Name = "http://my/api",
                Scopes = new()
                {
                    new ApiResourceScope
                    {
                        Scope = "api-access",
                    }
                }
            });

            cfgDbx.Clients.Add(new Client
            {
                ClientId = "api-client",
                AllowedGrantTypes = new()
                {
                    new ClientGrantType
                    {
                        GrantType = IdentityServer4.Models.GrantType.ClientCredentials
                    }
                },
                AllowedScopes = new()
                {
                    new ClientScope
                    {
                        Scope = "api-access"
                    }
                },
                RequireClientSecret = true,
                ClientSecrets = new()
                {
                    new ClientSecret
                    {
                        Value = IdentityServer4.Models.HashExtensions.Sha256("secret")
                    }
                }
            });

            await cfgDbx.SaveChangesAsync();

            var client = this.IdentityShell.CreateClient();
            var discoveryDocument = await client.GetDiscoveryDocumentAsync();

            // ACT

            var result = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "api-client",
                ClientSecret = "secret",
                Scope = "api-access",
            });

            Assert.False(result.IsError);
            Assert.NotNull(result.AccessToken);
        }

        [Fact]
        public async Task Configure_resource_owner_and_fetch_token()
        {
            // ARRANGE

            using var cfgDbx = this.IdentityShell.Services.GetRequiredService<ConfigurationDbContext>();

            cfgDbx.ApiScopes.Add(new ApiScope
            {
                Name = "api-access"
            });

            cfgDbx.ApiResources.Add(new ApiResource
            {
                Name = "http://my/api",
                Scopes = new()
                {
                    new ApiResourceScope
                    {
                        Scope = "api-access",
                    }
                }
            });

            cfgDbx.IdentityResources.Add(new IdentityServer4.Models.IdentityResources.OpenId().ToEntity());
            cfgDbx.IdentityResources.Add(new IdentityServer4.Models.IdentityResources.Profile().ToEntity());

            cfgDbx.Clients.Add(new Client
            {
                ClientId = "ro-client",
                AllowedGrantTypes = new()
                {
                    new ClientGrantType { GrantType = IdentityServer4.Models.GrantType.ResourceOwnerPassword },
                    new ClientGrantType { GrantType = IdentityServer4.Models.GrantType.ClientCredentials }
                },
                AllowedScopes = new()
                {
                    new ClientScope { Scope = "api-access" },
                    new ClientScope { Scope = "openid" },
                    new ClientScope { Scope = "profile" },
                },
                RequireClientSecret = true,
                ClientSecrets = new()
                {
                    new ClientSecret
                    {
                        Value = IdentityServer4.Models.HashExtensions.Sha256("secret")
                    }
                }
            });

            await cfgDbx.SaveChangesAsync();

            var client = this.IdentityShell.CreateClient();

            var userManager = this.IdentityShell.Services.GetRequiredService<UserManager<ApplicationUser>>();

            var result1 = await userManager.CreateAsync(
                user: new ApplicationUser
                {
                    UserName = "alice",
                },
                password: "Password123$");

            var aliceUser = await userManager.FindByNameAsync("alice");

            await userManager.AddClaimAsync(aliceUser, new Claim("name", "Alice Smith"));
            await userManager.AddClaimAsync(aliceUser, new Claim("given_name", "Alice"));
            await userManager.AddClaimAsync(aliceUser, new Claim("family_name", "Smith"));
            await userManager.AddClaimAsync(aliceUser, new Claim("email", "AliceSmith@email.com"));
            await userManager.AddClaimAsync(aliceUser, new Claim("email_verified", "true"));
            await userManager.AddClaimAsync(aliceUser, new Claim("website", "http://alice.com"));

            var discoveryDocument = await client.GetDiscoveryDocumentAsync();

            // ACT

            var result = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "ro-client",
                ClientSecret = "secret",
                Scope = "api-access openid profile",
                UserName = "alice",
                Password = "Password123$",
            });

            Assert.False(result.IsError);
            Assert.NotNull(result.AccessToken);
        }

        [Fact]
        public async Task Configure_resource_owner_and_fetch_userinfo()
        {
            // ARRANGE

            using var cfgDbx = this.IdentityShell.Services.GetRequiredService<ConfigurationDbContext>();

            cfgDbx.ApiScopes.Add(new ApiScope
            {
                Name = "api-access"
            });

            cfgDbx.ApiResources.Add(new ApiResource
            {
                Name = "http://my/api",
                Scopes = new()
                {
                    new ApiResourceScope
                    {
                        Scope = "api-access",
                    }
                }
            });

            cfgDbx.IdentityResources.Add(new IdentityServer4.Models.IdentityResources.OpenId().ToEntity());
            cfgDbx.IdentityResources.Add(new IdentityServer4.Models.IdentityResources.Profile().ToEntity());

            cfgDbx.Clients.Add(new Client
            {
                ClientId = "ro-client",
                AllowedGrantTypes = new()
                {
                    new ClientGrantType { GrantType = IdentityServer4.Models.GrantType.ResourceOwnerPassword },
                    new ClientGrantType { GrantType = IdentityServer4.Models.GrantType.ClientCredentials }
                },
                AllowedScopes = new()
                {
                    new ClientScope { Scope = "api-access" },
                    new ClientScope { Scope = "openid" },
                    new ClientScope { Scope = "profile" },
                },
                RequireClientSecret = true,
                ClientSecrets = new()
                {
                    new ClientSecret
                    {
                        Value = IdentityServer4.Models.HashExtensions.Sha256("secret")
                    }
                }
            });

            await cfgDbx.SaveChangesAsync();

            var client = this.IdentityShell.CreateClient();

            var userManager = this.IdentityShell.Services.GetRequiredService<UserManager<ApplicationUser>>();

            var result1 = await userManager.CreateAsync(
                user: new ApplicationUser
                {
                    UserName = "alice",
                },
                password: "Password123$");

            var aliceUser = await userManager.FindByNameAsync("alice");

            await userManager.AddClaimAsync(aliceUser, new Claim("name", "Alice Smith"));
            await userManager.AddClaimAsync(aliceUser, new Claim("given_name", "Alice"));
            await userManager.AddClaimAsync(aliceUser, new Claim("family_name", "Smith"));
            await userManager.AddClaimAsync(aliceUser, new Claim("email", "AliceSmith@email.com"));
            await userManager.AddClaimAsync(aliceUser, new Claim("email_verified", "true"));
            await userManager.AddClaimAsync(aliceUser, new Claim("website", "http://alice.com"));

            var discoveryDocument = await client.GetDiscoveryDocumentAsync();

            var accessToken = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "ro-client",
                ClientSecret = "secret",
                Scope = "api-access openid profile",
                UserName = "alice",
                Password = "Password123$",
            });

            // ACT

            var result = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = discoveryDocument.UserInfoEndpoint,
                Token = accessToken.AccessToken,
            });

            // ASSERT

            Assert.False(result.IsError);
            Assert.Equal("Alice Smith", result.Json["name"]);
            Assert.Equal("Alice", result.Json["given_name"]);
            Assert.Equal("Smith", result.Json["family_name"]);
            Assert.Equal("alice", result.Json["preferred_username"]);
            Assert.Equal("http://alice.com", result.Json["website"]);
        }
    }
}