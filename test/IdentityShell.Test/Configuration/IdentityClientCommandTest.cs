using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using IdentityShell.Cmdlets.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security.Claims;
using Xunit;

namespace IdentityShell.Test
{
    [Collection(nameof(ConfigurationDbContext))]
    public class IdentityClientCommandTest : IdentityConfigurationCommandTestBase
    {
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

        private void AssertClient(Client client)
        {
            Assert.Equal("client-id", client.ClientId);
            Assert.True(client.AllowOfflineAccess);
            Assert.Equal(1, client.IdentityTokenLifetime);
            Assert.Equal(2, client.AccessTokenLifetime);
            Assert.Equal(3, client.AuthorizationCodeLifetime);
            Assert.Equal(4, client.AbsoluteRefreshTokenLifetime);
            Assert.Equal(5, client.SlidingRefreshTokenLifetime);
            Assert.Equal(6, client.ConsentLifetime);
            Assert.Equal(TokenUsage.OneTimeOnly, client.RefreshTokenUsage);
            Assert.True(client.UpdateAccessTokenClaimsOnRefresh);
            Assert.Equal(TokenExpiration.Absolute, client.RefreshTokenExpiration);
            Assert.Equal(AccessTokenType.Reference, client.AccessTokenType);
            Assert.True(client.EnableLocalLogin);
            Assert.Equal(Array("ipr-1", "ipr-2"), client.IdentityProviderRestrictions);
            Assert.True(client.IncludeJwtId);

            Assert.True(client.AlwaysSendClientClaims);
            Assert.Equal("prefix", client.ClientClaimsPrefix);
            Assert.Equal("subjectsalt", client.PairWiseSubjectSalt);
            Assert.Equal(1, client.UserSsoLifetime);
            Assert.Equal("usercodetype", client.UserCodeType);
            Assert.Equal(7, client.DeviceCodeLifetime);
            Assert.True(client.AlwaysIncludeUserClaimsInIdToken);
            Assert.Equal(Array("scope-1", "scope-2"), client.AllowedScopes);
            Assert.Equal(new Dictionary<string, string>()
            {
                { "p1", "v1" },
                { "p2", "v2" }
            }, client.Properties);
            Assert.True(client.BackChannelLogoutSessionRequired);
            Assert.True(client.Enabled);
            Assert.Equal("client-id", client.ClientId);
            Assert.Equal("protocolType", client.ProtocolType);
            Assert.True(client.RequireClientSecret);
            Assert.Equal("clientname", client.ClientName);
            Assert.Equal("description", client.Description);
            Assert.Equal("clienturi", client.ClientUri);
            Assert.Equal("logouri", client.LogoUri);
            Assert.Equal(Array("o1", "o2"), client.AllowedCorsOrigins);
            Assert.True(client.RequireConsent);
            Assert.Equal(Array(GrantType.DeviceFlow, GrantType.Hybrid), client.AllowedGrantTypes);
            Assert.True(client.RequirePkce);
            Assert.True(client.AllowPlainTextPkce);
            Assert.True(client.AllowAccessTokensViaBrowser);
            Assert.Equal(Array("redirect-1", "redirect-2"), client.RedirectUris);
            Assert.Equal(Array("plredirect-1", "plredirect-2"), client.PostLogoutRedirectUris);
            Assert.Equal("frontChannelLogoutUri", client.FrontChannelLogoutUri);
            Assert.True(client.FrontChannelLogoutSessionRequired);
            Assert.Equal("backChannelLogoutUri", client.BackChannelLogoutUri);
            Assert.True(client.AllowRememberConsent);

            Assert.Equal("value", client.ClientSecrets.Single().Value);
            Assert.Equal("description", client.ClientSecrets.Single().Description);
            //            Assert.Equal(clientSecretExpiration, resultValue.ClientSecrets.Single().Expiration);
            Assert.Equal("SharedSecret", client.ClientSecrets.Single().Type);

            Assert.Equal("type", client.Claims.Single().Type);
            Assert.Equal("value", client.Claims.Single().Value);
            Assert.Equal(ClaimValueTypes.String, client.Claims.Single().ValueType);
        }

        [Fact]
        public void IdentityShell_creates_Client()
        {
            // ACT

            this.PowerShell
                .AddCommandEx<SetIdentityClientCommand>(cmd =>
                {
                    cmd
                        .AddParameter(c => c.ClientId, "client-id")
                        .AddParameter(c => c.AllowOfflineAccess, true)
                        .AddParameter(c => c.IdentityTokenLifetime, 1)
                        .AddParameter(c => c.AccessTokenLifetime, 2)
                        .AddParameter(c => c.AuthorizationCodeLifetime, 3)
                        .AddParameter(c => c.AbsoluteRefreshTokenLifetime, 4)
                        .AddParameter(c => c.SlidingRefreshTokenLifetime, 5)
                        .AddParameter(c => c.ConsentLifetime, 6)
                        .AddParameter(c => c.RefreshTokenUsage, TokenUsage.OneTimeOnly)
                        .AddParameter(c => c.UpdateAccessTokenClaimsOnRefresh, true)
                        .AddParameter(c => c.RefreshTokenExpiration, TokenExpiration.Absolute)
                        .AddParameter(c => c.AccessTokenType, AccessTokenType.Reference)
                        .AddParameter(c => c.EnableLocalLogin, true)
                        .AddParameter(c => c.IdentityProviderRestrictions, Array("ipr-1", "ipr-2"))
                        .AddParameter(c => c.IncludeJwtId, true)
                        .AddParameter(c => c.Claims, Array(new ClientClaim("type", "value", ClaimValueTypes.String)))
                        .AddParameter(c => c.AlwaysSendClientClaims, true)
                        .AddParameter(c => c.ClientClaimsPrefix, "prefix")
                        .AddParameter(c => c.PairWiseSubjectSalt, "subjectsalt")
                        .AddParameter(c => c.UserSsoLifetime, 1)
                        .AddParameter(c => c.UserCodeType, "usercodetype")
                        .AddParameter(c => c.DeviceCodeLifetime, 7)
                        .AddParameter(c => c.AlwaysIncludeUserClaimsInIdToken, true)
                        .AddParameter(c => c.AllowedScopes, Array("scope-1", "scope-2"))
                        .AddParameter(c => c.Properties, new Hashtable
                        {
                            { "p1", "v1" },
                            { "p2", "v2" }
                        })
                        .AddParameter(c => c.BackChannelLogoutSessionRequired, true)
                        .AddParameter(c => c.Enabled, true)
                        .AddParameter(c => c.ProtocolType, "protocolType")
                        .AddParameter(c => c.ClientSecrets, Array(new Secret("value", "description", DateTime.Now)))
                        .AddParameter(c => c.RequireClientSecret, true)
                        .AddParameter(c => c.ClientName, "clientname")
                        .AddParameter(c => c.Description, "description")
                        .AddParameter(c => c.ClientUri, "clienturi")
                        .AddParameter(c => c.LogoUri, "logouri")
                        .AddParameter(c => c.AllowedCorsOrigins, Array("o1", "o2"))
                        .AddParameter(c => c.RequireConsent, true)
                        .AddParameter(c => c.AllowedGrantTypes, Array(GrantType.DeviceFlow, GrantType.Hybrid))
                        .AddParameter(c => c.RequirePkce, true)
                        .AddParameter(c => c.AllowPlainTextPkce, true)
                        .AddParameter(c => c.AllowAccessTokensViaBrowser, true)
                        .AddParameter(c => c.RedirectUris, Array("redirect-1", "redirect-2"))
                        .AddParameter(c => c.PostLogoutRedirectUris, Array("plredirect-1", "plredirect-2"))
                        .AddParameter(c => c.FrontChannelLogoutUri, "frontChannelLogoutUri")
                        .AddParameter(c => c.FrontChannelLogoutSessionRequired, true)
                        .AddParameter(c => c.BackChannelLogoutUri, "backChannelLogoutUri")
                        .AddParameter(c => c.AllowRememberConsent, true);
                });

            var result = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            AssertClient(result.As<Client>());
        }

        private PSObject ArrangeClient(DateTime clientSecretExpiration)
        {
            this.PowerShell
               .AddCommandEx<SetIdentityClientCommand>(cmd =>
               {
                   cmd
                       .AddParameter(c => c.ClientId, "client-id")
                       .AddParameter(c => c.AllowOfflineAccess, true)
                       .AddParameter(c => c.IdentityTokenLifetime, 1)
                       .AddParameter(c => c.AccessTokenLifetime, 2)
                       .AddParameter(c => c.AuthorizationCodeLifetime, 3)
                       .AddParameter(c => c.AbsoluteRefreshTokenLifetime, 4)
                       .AddParameter(c => c.SlidingRefreshTokenLifetime, 5)
                       .AddParameter(c => c.ConsentLifetime, 6)
                       .AddParameter(c => c.RefreshTokenUsage, TokenUsage.OneTimeOnly)
                       .AddParameter(c => c.UpdateAccessTokenClaimsOnRefresh, true)
                       .AddParameter(c => c.RefreshTokenExpiration, TokenExpiration.Absolute)
                       .AddParameter(c => c.AccessTokenType, AccessTokenType.Reference)
                       .AddParameter(c => c.EnableLocalLogin, true)
                       .AddParameter(c => c.IdentityProviderRestrictions, Array("ipr-1", "ipr-2"))
                       .AddParameter(c => c.IncludeJwtId, true)
                       .AddParameter(c => c.Claims, Array(new ClientClaim("type", "value", "valueType")))
                       .AddParameter(c => c.AlwaysSendClientClaims, true)
                       .AddParameter(c => c.ClientClaimsPrefix, "prefix")
                       .AddParameter(c => c.PairWiseSubjectSalt, "subjectsalt")
                       .AddParameter(c => c.UserSsoLifetime, 1)
                       .AddParameter(c => c.UserCodeType, "usercodetype")
                       .AddParameter(c => c.DeviceCodeLifetime, 7)
                       .AddParameter(c => c.AlwaysIncludeUserClaimsInIdToken, true)
                       .AddParameter(c => c.AllowedScopes, Array("scope-1", "scope-2"))
                       .AddParameter(c => c.Properties, new Hashtable
                       {
                            { "p1", "v1" },
                            { "p2", "v2" }
                       })
                       .AddParameter(c => c.BackChannelLogoutSessionRequired, true)
                       .AddParameter(c => c.Enabled, true)
                       .AddParameter(c => c.ProtocolType, "protocolType")
                       .AddParameter(c => c.ClientSecrets, Array(new Secret("value", "description", DateTime.Now)))
                       .AddParameter(c => c.RequireClientSecret, true)
                       .AddParameter(c => c.ClientName, "clientname")
                       .AddParameter(c => c.Description, "description")
                       .AddParameter(c => c.ClientUri, "clienturi")
                       .AddParameter(c => c.LogoUri, "logouri")
                       .AddParameter(c => c.AllowedCorsOrigins, Array("o1", "o2"))
                       .AddParameter(c => c.RequireConsent, true)
                       .AddParameter(c => c.AllowedGrantTypes, Array(GrantType.DeviceFlow, GrantType.Hybrid))
                       .AddParameter(c => c.RequirePkce, true)
                       .AddParameter(c => c.AllowPlainTextPkce, true)
                       .AddParameter(c => c.AllowAccessTokensViaBrowser, true)
                       .AddParameter(c => c.RedirectUris, Array("redirect-1", "redirect-2"))
                       .AddParameter(c => c.PostLogoutRedirectUris, Array("plredirect-1", "plredirect-2"))
                       .AddParameter(c => c.FrontChannelLogoutUri, "frontChannelLogoutUri")
                       .AddParameter(c => c.FrontChannelLogoutSessionRequired, true)
                       .AddParameter(c => c.BackChannelLogoutUri, "backChannelLogoutUri")
                       .AddParameter(c => c.AllowRememberConsent, true);
               });

            var client = this.PowerShell.Invoke().Single(); ;
            this.PowerShell.Commands.Clear();
            return client;
        }

        [Fact]
        public void IdentityShell_reads_Clients()
        {
            // ARRANGE

            var clientSecretExpiration = DateTime.Now;

            ArrangeClient(clientSecretExpiration);

            // ACT

            this.PowerShell.AddCommandEx<GetIdentityClientCommand>();

            var result = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            AssertClient(result.As<Client>());
        }

        [Fact]
        public void IdentityShell_reads_Clients_by_clientid()
        {
            // ARRANGE

            var clientSecretExpiration = DateTime.Now;

            var pso = ArrangeClient(clientSecretExpiration);

            // ACT

            this.PowerShell.AddCommandEx<GetIdentityClientCommand>(cmdlet => cmdlet.AddParameter(c => c.ClientId, pso.As<Client>().ClientId));

            var result = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            AssertClient(result.As<Client>());
        }

        [Fact]
        public void IdentityShell_modifies_piped_Client()
        {
            var clientSecretExpiration = DateTime.Now;
            var pso = ArrangeClient(clientSecretExpiration);

            // ACT

            var result = this.PowerShell
                .AddCommandEx<SetIdentityClientCommand>(cmdlet => cmdlet.AddParameter(c => c.RequireConsent, false))
                .Invoke(Array(pso))
                .Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            Assert.False(result.As<Client>().RequireConsent);
        }

        [Fact]
        public void IdentityShell_removes_client()
        {
            // ARRANGE

            var clientSecretExpiration = DateTime.Now;
            var pso = ArrangeClient(clientSecretExpiration);

            // ACT

            this.PowerShell
                .AddCommand("Remove-IdentityClient")
                    .AddParameter("ClientId", "client-id");

            this.PowerShell.Invoke();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();

            Assert.Empty(this.PowerShell.AddCommand("Get-IdentityClient").Invoke().ToArray());
        }

        [Fact]
        public void IdentityShell_removes_client_from_pipe()
        {
            // ARRANGE

            var clientSecretExpiration = DateTime.Now;
            ArrangeClient(clientSecretExpiration);

            // ACT

            this.PowerShell
                .AddCommand("Get-IdentityClient")
                .AddCommand("Remove-IdentityClient");
            this.PowerShell.Invoke();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();

            Assert.Empty(this.PowerShell.AddCommand("Get-IdentityClient").Invoke().ToArray());
        }
    }
}