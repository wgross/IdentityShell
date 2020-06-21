using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Xunit;

namespace IdentityShell.Test
{
    [Collection(nameof(ConfigurationDbContext))]
    public class IdentityApiResourceCommandTest : IdentityConfigurationCommandTestBase
    {
        private PSObject ArrangeIdentityApiResource(DateTime secretExpiration)
        {
            this.PowerShell
                   .AddCommand("Set-IdentityApiResource")
                        .AddParameter("Name", "name")
                        .AddParameter("DisplayName", "displayName")
                        .AddParameter("Description", "description")
                        .AddParameter("UserClaims", Array("claim-1", "claim-2"))
                        .AddParameter("Properties", new Hashtable
                        {
                            {"p1", "v1" },
                            {"p2", "v2" }
                        })
                        .AddParameter("ApiSecrets", new[]
                        {
                            new Secret("value", "description", secretExpiration)
                        })
                        .AddParameter("Scopes", new[]
                        {
                            new Scope("name", "displayName", new[]{"claimType" })
                            {
                                Description = "description",
                                Emphasize = true,
                                Required = true,
                                ShowInDiscoveryDocument = true
                            }
                        });

            var pso = this.PowerShell.Invoke().Single();
            this.PowerShell.Commands.Clear();
            return pso;
        }

        [Fact]
        public void IdentityShell_reads_empty_api_table()
        {
            // ACT

            this.PowerShell
                .AddCommand("Get-IdentityApiResource");

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Empty(result);
        }

        [Fact]
        public void IdentityShell_creates_api()
        {
            // ACT
            var secretExpiration = DateTime.Now;

            this.PowerShell
                .AddCommand("Set-IdentityApiResource")
                    .AddParameter("Name", "name")
                    .AddParameter("DisplayName", "displayName")
                    .AddParameter("Description", "description")
                    .AddParameter("UserClaims", Array("claim-1", "claim-2"))
                    .AddParameter("Properties", new Hashtable
                    {
                    {"p1", "v1" },
                    {"p2", "v2" }
                    })
                    .AddParameter("ApiSecrets", new[]
                    {
                        new Secret("value", "description", secretExpiration)
                    })
                    .AddParameter("Scopes", new[]
                    {
                        new Scope("name", "displayName", new[]{"claimType" })
                        {
                            Description = "description",
                            Emphasize = true,
                            Required = true,
                            ShowInDiscoveryDocument = true
                        }
                    });

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Single(result);
        }

        [Fact]
        public void IdentityShell_reads_all_apis()
        {
            // ARRANGE
            var secretExpiration = DateTime.Now;
            var pso = ArrangeIdentityApiResource(secretExpiration);

            // ACT

            this.PowerShell.AddCommand("Get-IdentityApiResource");
            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.Single();

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

            Assert.Equal("name", resultValue.Property<ICollection<Scope>>("Scopes").Single().Name);
            Assert.Equal("description", resultValue.Property<ICollection<Scope>>("Scopes").Single().Description);
            Assert.Equal("displayName", resultValue.Property<ICollection<Scope>>("Scopes").Single().DisplayName);
            Assert.True(resultValue.Property<ICollection<Scope>>("Scopes").Single().Emphasize);
            Assert.True(resultValue.Property<ICollection<Scope>>("Scopes").Single().ShowInDiscoveryDocument);
            Assert.True(resultValue.Property<ICollection<Scope>>("Scopes").Single().Required);
        }

        [Fact]
        public void IdentityShell_modifies_piped_api()
        {
            var secretExpiration = DateTime.Now;
            var pso = ArrangeIdentityApiResource(secretExpiration);

            // ACT

            this.PowerShell
                .AddCommand("Get-IdentityApiResource")
                .AddCommand("Set-IdentityApiResource")
                    .AddParameter("DisplayName", "displayname-changed");

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.Single();

            Assert.Equal("displayname-changed", resultValue.Property<string>("DisplayName"));
        }

        [Fact]
        public void IdentityShell_removes_api()
        {
            // ARRANGE

            var secretExpiration = DateTime.Now;
            var pso = ArrangeIdentityApiResource(secretExpiration);

            // ACT

            this.PowerShell
                .AddCommand("Remove-IdentityApiResource")
                    .AddParameter("Name", "name");

            this.PowerShell.Invoke();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();

            Assert.Empty(this.PowerShell.AddCommand("Get-IdentityApiResource").Invoke().ToArray());
        }

        [Fact]
        public void IdentityShell_removes_api_from_pipe()
        {
            // ARRANGE

            var secretExpiration = DateTime.Now;
            ArrangeIdentityApiResource(secretExpiration);

            // ACT

            this.PowerShell
                .AddCommand("Get-IdentityApiResource")
                .AddCommand("Remove-IdentityApiResource");
            this.PowerShell.Invoke();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();

            Assert.Empty(this.PowerShell.AddCommand("Get-IdentityApiResource").Invoke().ToArray());
        }

        [Fact]
        public void IdentityShell_creates_api_from_piped_model()
        {
            // ARRANGE

            var model = new ApiResource("api1", "My API");

            // ACT

            this.PowerShell
                .AddCommand("Set-IdentityApiResource");

            this.PowerShell.Invoke(new object[] { model }).Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();
            var result = this.PowerShell.AddCommand("Get-IdentityApiResource").Invoke().Single();

            Assert.Equal(model.Description, result.Property<string>("Description"));
            Assert.Equal(model.DisplayName, result.Property<string>("DisplayName"));
            Assert.Equal(model.Enabled, result.Property<bool>("Enabled"));
            Assert.Equal(model.ApiSecrets, result.Property<ICollection<Secret>>("ApiSecrets"));
            Assert.Equal(model.Name, result.Property<string>("Name"));
            Assert.Equal(model.Properties, result.Property<IDictionary<string, string>>("Properties"));
            Assert.Equal("api1", result.Property<ICollection<Scope>>("Scopes").Single().Name);
            Assert.Null(result.Property<ICollection<Scope>>("Scopes").Single().Description);
            Assert.Equal("My API", result.Property<ICollection<Scope>>("Scopes").Single().DisplayName);
            Assert.False(result.Property<ICollection<Scope>>("Scopes").Single().Emphasize);
            Assert.True(result.Property<ICollection<Scope>>("Scopes").Single().ShowInDiscoveryDocument);
            Assert.False(result.Property<ICollection<Scope>>("Scopes").Single().Required);
            Assert.Equal(model.UserClaims, result.Property<ICollection<string>>("UserClaims"));
        }
    }
}