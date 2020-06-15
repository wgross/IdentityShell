using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using IdentityShell.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Security.Claims;
using Xunit;

namespace IdentityShell.Test
{
    public class IdentityClientCommandTest : IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private readonly string connectionString;

        public PowerShell PowerShell { get; }

        public IdentityClientCommandTest()
        {
            string createConnectionString(Guid instanceId, string path) => $@"Data Source=(LocalDb)\MSSQLLocalDB;database=IdentityShell.Test.{instanceId};trusted_connection=yes;AttachDBFilename={path}\IdentityShell-{instanceId}.mdf";

            this.connectionString = createConnectionString(
                instanceId: Guid.NewGuid(),
                path: Path.GetDirectoryName(typeof(IdentityClientCommandTest).GetTypeInfo().Assembly.Location));

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddIdentityServer()
                .AddConfigurationStore(options =>
                {
                    string migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

                    options.ConfigureDbContext = builder => builder.UseSqlServer(this.connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                });

            this.serviceProvider = serviceCollection.BuildServiceProvider();

            using var serviceScope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            context.Database.EnsureCreated();

            IdentityCommandBase.ServiceProvider = this.serviceProvider;
            this.PowerShell = PowerShell.Create(InitialSessionState.CreateDefault().AddIdentityCommands());
        }

        public void Dispose()
        {
            this.serviceProvider
                .CreateScope()
                    .ServiceProvider
                    .GetRequiredService<ConfigurationDbContext>()
                        .Database
                        .EnsureDeleted();
        }

        [Fact]
        public void IdentityShell_read_empty_client_table()
        {
            // ACT

            this.PowerShell
                .AddCommand("Get-IdentityClient");

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Empty(result);
        }

        [Fact]
        public void IdentityShell_creates_client()
        {
            // ACT

            this.PowerShell
                .AddCommand("Set-IdentityClient")
                    .AddParameter("ClientId", "client-id")
                    .AddParameter("AllowOfflineAccess", true)
                    .AddParameter("IdentityTokenLifetime", 1)
                    .AddParameter("AccessTokenLifetime", 2)
                    .AddParameter("AuthorizationCodeLifetime", 3)
                    .AddParameter("AbsoluteRefreshTokenLifetime", 4)
                    .AddParameter("SlidingRefreshTokenLifetime", 5)
                    .AddParameter("ConsentLifetime", 6)
                    .AddParameter("RefreshTokenUsage", TokenUsage.OneTimeOnly)
                    .AddParameter("UpdateAccessTokenClaimsOnRefresh", true)
                    .AddParameter("RefreshTokenExpiration", TokenExpiration.Absolute)
                    .AddParameter("AccessTokenType", AccessTokenType.Reference)
                    .AddParameter("EnableLocalLogin", true)
                    .AddParameter("IdentityProviderRestrictions", new[] { "ipr-1", "ipr-2" })
                    .AddParameter("IncludeJwtId", true)
                    .AddParameter("Claims", new[]
                    {
                        new Claim("type", "value", "valueType", "issuer", "originaIssuer", new ClaimsIdentity("authenticationType", "nameType", "roleType"))
                    })
                    .AddParameter("AlwaysSendClientClaims", true)
                    .AddParameter("ClientClaimsPrefix", "prefix")
                    .AddParameter("PairWiseSubjectSalt", "subjectsalt")
                    .AddParameter("UserSsoLifetime", 1)
                    .AddParameter("UserCodeType", "usercodetype")
                    .AddParameter("DeviceCodeLifeTime", 7)
                    .AddParameter("AlwaysIncludeUserClaimsInIdToken", true)
                    .AddParameter("AllowedScopes", new[] { "scope-1", "scope-2" })
                    .AddParameter("Properties", new Hashtable
                    {
                        { "p1", "v1" },
                        { "p2", "v2" }
                    })
                    .AddParameter("BackChannelLogoutSessionRequired", true)
                    .AddParameter("Enabled", true)
                    .AddParameter("ProtocolType", "protocolType")
                    .AddParameter("ClientSecrets", new[] { new Secret("value", "description", DateTime.Now) })
                    .AddParameter("RequireClientSecret", true)
                    .AddParameter("ClientName", "clientname")
                    .AddParameter("Description", "description")
                    .AddParameter("ClientUri", "clienturi")
                    .AddParameter("LogoUri", "logouri")
                    .AddParameter("AllowedCorsOrigins", new[] { "o1", "o2" })
                    .AddParameter("RequireConsent", true)
                    .AddParameter("AllowedGrantTypes", new[] { "grant1", "grant2" })
                    .AddParameter("RequirePkce", true)
                    .AddParameter("AllowPlainTextPkce", true)
                    .AddParameter("AllowAccessTokensViaBrowser", true)
                    .AddParameter("RedirectUris", new[] { "redirect1", "redirect2" })
                    .AddParameter("PostLogoutRedirectUris", new[] { "plredirect", "plredirect" })
                    .AddParameter("FrontChannelLogoutUri", "frontChannelLogoutUri")
                    .AddParameter("FrontChannelLogoutSessionRequired", true)
                    .AddParameter("BackChannelLogoutUri", "backChannelLogoutUri")
                    .AddParameter("AllowRememberConsent", true);

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Single(result);
        }

        [Fact]
        public void IdentityShell_reads_all_clients()
        {
            // ARRANGE

            var clientSecretExpiration = DateTime.Now;

            ArrangeClient(clientSecretExpiration);

            // ACT

            this.PowerShell.AddCommand("Get-IdentityClient");

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.Single();

            Assert.Equal("client-id", resultValue.Property<string>("ClientId"));
            Assert.True(resultValue.Property<bool>("AllowOfflineAccess"));
            Assert.Equal(1, resultValue.Property<int>("IdentityTokenLifetime"));
            Assert.Equal(2, resultValue.Property<int>("AccessTokenLifetime"));
            Assert.Equal(3, resultValue.Property<int>("AuthorizationCodeLifetime"));
            Assert.Equal(4, resultValue.Property<int>("AbsoluteRefreshTokenLifetime"));
            Assert.Equal(5, resultValue.Property<int>("SlidingRefreshTokenLifetime"));
            Assert.Equal(6, resultValue.Property<int>("ConsentLifetime"));
            Assert.Equal(TokenUsage.OneTimeOnly, resultValue.Property<TokenUsage>("RefreshTokenUsage"));
            Assert.True(resultValue.Property<bool>("UpdateAccessTokenClaimsOnRefresh"));
            Assert.Equal(TokenExpiration.Absolute, resultValue.Property<TokenExpiration>("RefreshTokenExpiration"));
            Assert.Equal(AccessTokenType.Reference, resultValue.Property<AccessTokenType>("AccessTokenType"));
            Assert.True(resultValue.Property<bool>("EnableLocalLogin"));
            Assert.Equal(new[] { "ipr-1", "ipr-2" }, resultValue.Property<ICollection<string>>("IdentityProviderRestrictions"));
            Assert.True(resultValue.Property<bool>("IncludeJwtId"));

            Assert.True(resultValue.Property<bool>("AlwaysSendClientClaims"));
            Assert.Equal("prefix", resultValue.Property<string>("ClientClaimsPrefix"));
            Assert.Equal("subjectsalt", resultValue.Property<string>("PairWiseSubjectSalt"));
            Assert.Equal(1, resultValue.Property<int?>("UserSsoLifetime"));
            Assert.Equal("usercodetype", resultValue.Property<string>("UserCodeType"));
            Assert.Equal(7, resultValue.Property<int>("DeviceCodeLifetime"));
            Assert.True(resultValue.Property<bool>("AlwaysIncludeUserClaimsInIdToken"));
            Assert.Equal(new[] { "scope-1", "scope-2" }, resultValue.Property<ICollection<string>>("AllowedScopes"));
            Assert.Equal(new Dictionary<string, string>()
            {
                { "p1", "v1" },
                { "p2", "v2" }
            }, resultValue.Property<IDictionary<string, string>>("Properties"));
            Assert.True(resultValue.Property<bool>("BackChannelLogoutSessionRequired"));
            Assert.True(resultValue.Property<bool>("Enabled"));
            Assert.Equal("client-id", resultValue.Property<string>("ClientId"));
            Assert.Equal("protocolType", resultValue.Property<string>("ProtocolType"));
            Assert.True(resultValue.Property<bool>("RequireClientSecret"));
            Assert.Equal("clientname", resultValue.Property<string>("ClientName"));
            Assert.Equal("description", resultValue.Property<string>("Description"));
            Assert.Equal("clienturi", resultValue.Property<string>("ClientUri"));
            Assert.Equal("logouri", resultValue.Property<string>("LogoUri"));
            Assert.Equal(new[] { "o1", "o2" }, resultValue.Property<ICollection<string>>("AllowedCorsOrigins"));
            Assert.True(resultValue.Property<bool>("RequireConsent"));
            Assert.Equal(new[] { "grant1", "grant2" }, resultValue.Property<ICollection<string>>("AllowedGrantTypes"));
            Assert.True(resultValue.Property<bool>("RequirePkce"));
            Assert.True(resultValue.Property<bool>("AllowPlainTextPkce"));
            Assert.True(resultValue.Property<bool>("AllowAccessTokensViaBrowser"));
            Assert.Equal(new[] { "redirect1", "redirect2" }, resultValue.Property<ICollection<string>>("RedirectUris"));
            Assert.Equal(new[] { "plredirect", "plredirect" }, resultValue.Property<ICollection<string>>("PostLogoutRedirectUris"));
            Assert.Equal("frontChannelLogoutUri", resultValue.Property<string>("FrontChannelLogoutUri"));
            Assert.True(resultValue.Property<bool>("FrontChannelLogoutSessionRequired"));
            Assert.Equal("backChannelLogoutUri", resultValue.Property<string>("BackChannelLogoutUri"));
            Assert.True(resultValue.Property<bool>("AllowRememberConsent"));

            Assert.Equal("value", resultValue.Property<IEnumerable<Secret>>("ClientSecrets").Single().Value);
            Assert.Equal("description", resultValue.Property<IEnumerable<Secret>>("ClientSecrets").Single().Description);
            Assert.Equal(clientSecretExpiration, resultValue.Property<IEnumerable<Secret>>("ClientSecrets").Single().Expiration);
            Assert.Equal("type", resultValue.Property<IEnumerable<Secret>>("ClientSecrets").Single().Type);

            Assert.Equal("type", resultValue.Property<ICollection<Claim>>("Claims").Single().Type);
            Assert.Equal("value", resultValue.Property<ICollection<Claim>>("Claims").Single().Value);
            Assert.Equal("http://www.w3.org/2001/XMLSchema#string", resultValue.Property<ICollection<Claim>>("Claims").Single().ValueType);
            Assert.Equal("LOCAL AUTHORITY", resultValue.Property<ICollection<Claim>>("Claims").Single().Issuer);
            Assert.Equal("LOCAL AUTHORITY", resultValue.Property<ICollection<Claim>>("Claims").Single().OriginalIssuer);
            Assert.Null(resultValue.Property<ICollection<Claim>>("Claims").Single().Subject); // ???
        }

        private PSObject ArrangeClient(DateTime clientSecretExpiration)
        {
            this.PowerShell
                .AddCommand("Set-IdentityClient")
                    .AddParameter("ClientId", "client-id")
                    .AddParameter("AllowOfflineAccess", true)
                    .AddParameter("IdentityTokenLifetime", 1)
                    .AddParameter("AccessTokenLifetime", 2)
                    .AddParameter("AuthorizationCodeLifetime", 3)
                    .AddParameter("AbsoluteRefreshTokenLifetime", 4)
                    .AddParameter("SlidingRefreshTokenLifetime", 5)
                    .AddParameter("ConsentLifetime", 6)
                    .AddParameter("RefreshTokenUsage", TokenUsage.OneTimeOnly)
                    .AddParameter("UpdateAccessTokenClaimsOnRefresh", true)
                    .AddParameter("RefreshTokenExpiration", TokenExpiration.Absolute)
                    .AddParameter("AccessTokenType", AccessTokenType.Reference)
                    .AddParameter("EnableLocalLogin", true)
                    .AddParameter("IdentityProviderRestrictions", new[] { "ipr-1", "ipr-2" })
                    .AddParameter("IncludeJwtId", true)
                    .AddParameter("Claims", new[]
                    {
                        new Claim("type", "value", "valueType", "issuer", "originaIssuer", new ClaimsIdentity("authenticationType", "nameType", "roleType"))
                    })
                    .AddParameter("AlwaysSendClientClaims", true)
                    .AddParameter("ClientClaimsPrefix", "prefix")
                    .AddParameter("PairWiseSubjectSalt", "subjectsalt")
                    .AddParameter("UserSsoLifetime", 1)
                    .AddParameter("UserCodeType", "usercodetype")
                    .AddParameter("DeviceCodeLifeTime", 7)
                    .AddParameter("AlwaysIncludeUserClaimsInIdToken", true)
                    .AddParameter("AllowedScopes", new[] { "scope-1", "scope-2" })
                    .AddParameter("Properties", new Hashtable
                    {
                        { "p1", "v1" },
                        { "p2", "v2" }
                    })
                    .AddParameter("BackChannelLogoutSessionRequired", true)
                    .AddParameter("Enabled", true)
                    .AddParameter("ProtocolType", "protocolType")
                    .AddParameter("ClientSecrets", new[]
                    {
                        new Secret("value", "description", clientSecretExpiration)
                        {
                            Type = "type"
                        }
                    })
                    .AddParameter("RequireClientSecret", true)
                    .AddParameter("ClientName", "clientname")
                    .AddParameter("Description", "description")
                    .AddParameter("ClientUri", "clienturi")
                    .AddParameter("LogoUri", "logouri")
                    .AddParameter("AllowedCorsOrigins", new[] { "o1", "o2" })
                    .AddParameter("RequireConsent", true)
                    .AddParameter("AllowedGrantTypes", new[] { "grant1", "grant2" })
                    .AddParameter("RequirePkce", true)
                    .AddParameter("AllowPlainTextPkce", true)
                    .AddParameter("AllowAccessTokensViaBrowser", true)
                    .AddParameter("RedirectUris", new[] { "redirect1", "redirect2" })
                    .AddParameter("PostLogoutRedirectUris", new[] { "plredirect", "plredirect" })
                    .AddParameter("FrontChannelLogoutUri", "frontChannelLogoutUri")
                    .AddParameter("FrontChannelLogoutSessionRequired", true)
                    .AddParameter("BackChannelLogoutUri", "backChannelLogoutUri")
                    .AddParameter("AllowRememberConsent", true);

            var client = this.PowerShell.Invoke().Single(); ;
            this.PowerShell.Commands.Clear();
            return client;
        }

        [Fact]
        public void IdentityShell_modifies_piped_client()
        {
            var clientSecretExpiration = DateTime.Now;
            var pso = ArrangeClient(clientSecretExpiration);

            // ACT

            this.PowerShell
                .AddCommand("Set-IdentityClient")
                    .AddParameter("RequireConsent", false);

            var result = this.PowerShell.Invoke(new[] { pso }).ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.Single();

            Assert.False(resultValue.Property<bool>("RequireConsent"));
            Assert.Same(pso.ImmediateBaseObject, resultValue.ImmediateBaseObject);
        }
    }
}