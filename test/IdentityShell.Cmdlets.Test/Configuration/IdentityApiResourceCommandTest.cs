using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using IdentityShell.Cmdlets;
using IdentityShell.Cmdlets.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Xunit;

namespace IdentityShell.Cmdlets.Test
{
    [Collection(nameof(IdentityCommandBase.GlobalServiceProvider))]
    public class IdentityApiResourceCommandTest : IdentityConfigurationCommandTestBase
    {
        private void AssertApiResource(PSObject resultValue)
        {
            Assert.True(resultValue.Property<bool>("Enabled"));
            Assert.Equal("name", resultValue.Property<string>("Name"));
            Assert.Equal("displayName", resultValue.Property<string>("DisplayName"));
            Assert.Equal("description", resultValue.Property<string>("Description"));
            Assert.Equal(new[] { "claim-1", "claim-2" }, resultValue.Property<ICollection<string>>("UserClaims"));
            Assert.Equal(new Dictionary<string, string>
            {
                {"p1", "v1" },
                {"p2", "v2" }
            },
            resultValue.Property<IDictionary<string, string>>("Properties"));

            Assert.Equal("name", resultValue.Property<List<string>>("Scopes").Single());
            //Assert.Equal("description", resultValue.Property<ICollection<ApiScope>>("Scopes").Single().Description);
            //Assert.Equal("displayName", resultValue.Property<ICollection<ApiScope>>("Scopes").Single().DisplayName);
            //Assert.True(resultValue.Property<ICollection<ApiScope>>("Scopes").Single().Emphasize);
            //Assert.True(resultValue.Property<ICollection<ApiScope>>("Scopes").Single().ShowInDiscoveryDocument);
            //Assert.True(resultValue.Property<ICollection<ApiScope>>("Scopes").Single().Required);
        }

        [Fact]
        public void IdentityShell_creates_api()
        {
            // ACT

            var apiScope = this.ArrangeApiScope();

            var secretExpiration = DateTime.Now;

            this.PowerShell
                .AddCommandEx<SetIdentityApiResourceCommand>(cmd =>
                {
                    cmd
                        .AddParameter(c => c.Name, "name")
                        .AddParameter(c => c.DisplayName, "displayName")
                        .AddParameter(c => c.Description, "description")
                        .AddParameter(c => c.UserClaims, Array("claim-1", "claim-2"))
                        .AddParameter(c => c.Properties, new Hashtable
                        {
                            { "p1", "v1" },
                            { "p2", "v2" }
                        })
                        .AddParameter(c => c.ApiSecrets, new[] { new Secret("value", "description", secretExpiration) })
                        .AddParameter(c => c.Scopes, new[] { apiScope.As<ApiScope>().Name });
                });

            var result = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            AssertApiResource(result);
        }

        private PSObject ArrangeIdentityApiResource(DateTime secretExpiration)
        {
            var apiScope = this.ArrangeApiScope();

            this.PowerShell
                .AddCommandEx<SetIdentityApiResourceCommand>(cmd =>
                {
                    cmd
                        .AddParameter(c => c.Name, "name")
                        .AddParameter(c => c.DisplayName, "displayName")
                        .AddParameter(c => c.Description, "description")
                        .AddParameter(c => c.UserClaims, Array("claim-1", "claim-2"))
                        .AddParameter(c => c.Properties, new Hashtable
                        {
                            { "p1", "v1" },
                            { "p2", "v2" }
                        })
                        .AddParameter(c => c.ApiSecrets, new[] { new Secret("value", "description", secretExpiration) })
                        .AddParameter(c => c.Scopes, new[] { apiScope.As<ApiScope>().Name });
                });

            var pso = this.PowerShell.Invoke().Single();
            this.PowerShell.Commands.Clear();
            return pso;
        }

        [Fact]
        public void IdentityShell_reads_ApiResources()
        {
            // ARRANGE
            var secretExpiration = DateTime.Now;
            var pso = ArrangeIdentityApiResource(secretExpiration);

            // ACT

            this.PowerShell.AddCommandEx<GetIdentityApiResourceCommand>();
            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.Single();

            AssertApiResource(resultValue);
        }

        [Fact]
        public void IdentityShell_reads_ApiResource_by_name()
        {
            // ARRANGE

            var secretExpiration = DateTime.Now;
            var pso = ArrangeIdentityApiResource(secretExpiration);

            // ACT

            this.PowerShell.AddCommandEx<GetIdentityApiResourceCommand>(cmd => cmd.AddParameter(c => c.Name, "name"));
            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.Single();

            AssertApiResource(resultValue);
        }

        [Fact]
        public void IdentityShell_modifies_piped_ApiResources()
        {
            var secretExpiration = DateTime.Now;
            var pso = ArrangeIdentityApiResource(secretExpiration);

            // ACT

            this.PowerShell
                .AddCommandEx<SetIdentityApiResourceCommand>(cmd => cmd.AddParameter(c => c.DisplayName, "displayname-changed"));

            var result = this.PowerShell.Invoke(Array(pso)).ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.Single();

            Assert.Equal("displayname-changed", resultValue.Property<string>("DisplayName"));
        }

        [Fact]
        public void IdentityShell_removes_ApiResource_by_name()
        {
            // ARRANGE

            var secretExpiration = DateTime.Now;
            var pso = ArrangeIdentityApiResource(secretExpiration);

            // ACT

            this.PowerShell
                .AddCommandEx<RemoveIdentityApiResourceCommand>(cmd => cmd.AddParameter(c => c.Name, "name"))
                .Invoke();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();

            Assert.Empty(this.PowerShell.AddCommandEx<GetIdentityApiResourceCommand>().Invoke().ToArray());
        }

        [Fact]
        public void IdentityShell_removes_ApiResource_from_pipe()
        {
            // ARRANGE

            var secretExpiration = DateTime.Now;
            var pso = ArrangeIdentityApiResource(secretExpiration);

            // ACT

            this.PowerShell
                .AddCommandEx<RemoveIdentityApiResourceCommand>()
                .Invoke(Array(pso));

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();

            Assert.Empty(this.PowerShell.AddCommandEx<GetIdentityApiResourceCommand>().Invoke().ToArray());
        }
    }
}