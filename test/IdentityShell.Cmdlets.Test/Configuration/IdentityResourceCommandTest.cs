using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using IdentityShell.Cmdlets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Xunit;

namespace IdentityShell.Cmdlets.Test.Configuration
{
    [Collection(nameof(IdentityCommandBase.GlobalServiceProvider))]
    public class IdentityResourceCommandTest : IdentityConfigurationCommandTestBase
    {
        private PSObject ArrangeIdentityResource()
        {
            this.PowerShell
                   .AddCommand("Set-IdentityResource")
                       .AddParameter("Name", "name")
                       .AddParameter("DisplayName", "displayName")
                       .AddParameter("Description", "description")
                       .AddParameter("ShowInDiscoveryDocument", true)
                       .AddParameter("UserClaims", Array("claim-1", "claim-2"))
                       .AddParameter("Properties", new Hashtable
                       {
                        {"p1", "v1" },
                        {"p2", "v2" }
                       })
                       .AddParameter("Required", true)
                       .AddParameter("Emphasize", true);

            var pso = this.PowerShell.Invoke().Single();
            this.PowerShell.Commands.Clear();
            return pso;
        }

        [Fact]
        public void IdentityShell_read_empty_user_table()
        {
            // ACT

            this.PowerShell
                .AddCommand("Get-IdentityResource");

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Empty(result);
        }

        [Fact]
        public void IdentityShell_creates_identity()
        {
            // ACT

            this.PowerShell
                .AddCommand("Set-IdentityResource")
                    .AddParameter("Name", "name")
                    .AddParameter("DisplayName", "displayName")
                    .AddParameter("Description", "description")
                    .AddParameter("ShowInDiscoveryDocument", true)
                    .AddParameter("UserClaims", Array("claim-1", "claim-2"))
                    .AddParameter("Properties", new Hashtable
                    {
                        {"p1", "v1" },
                        {"p2", "v2" }
                    })
                    .AddParameter("Required", true)
                    .AddParameter("Emphasize", true);

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Single(result);
        }

        [Fact]
        public void IdentityShell_reads_all_identities()
        {
            // ARRANGE

            var pso = ArrangeIdentityResource();

            // ACT

            this.PowerShell.AddCommand("Get-IdentityResource");
            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.Single();

            Assert.True(resultValue.Property<bool>("Enabled"));
            Assert.Equal("name", resultValue.Property<string>("Name"));
            Assert.Equal("displayName", resultValue.Property<string>("DisplayName"));
            Assert.Equal("description", resultValue.Property<string>("Description"));
            Assert.True(resultValue.Property<bool>("Required"));
            Assert.True(resultValue.Property<bool>("Emphasize"));
            Assert.True(resultValue.Property<bool>("ShowInDiscoveryDocument"));
            Assert.Equal(new[] { "claim-1", "claim-2" }, resultValue.Property<ICollection<string>>("UserClaims"));
            Assert.Equal(new Dictionary<string, string>
            {
                {"p1", "v1" },
                {"p2", "v2" }
            },
            resultValue.Property<IDictionary<string, string>>("Properties"));
        }

        [Fact]
        public void IdentityShell_modifies_piped_identity()
        {
            var pso = ArrangeIdentityResource();

            // ACT

            this.PowerShell
                .AddCommand("Set-IdentityResource")
                    .AddParameter("Required", false);

            var result = this.PowerShell.Invoke(new[] { pso }).ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.Single();

            Assert.False(resultValue.Property<bool>("Required"));
            Assert.Same(pso.ImmediateBaseObject, resultValue.ImmediateBaseObject);
        }

        [Fact]
        public void IdentityShell_removes_identity()
        {
            // ARRANGE

            var pso = ArrangeIdentityResource();

            // ACT

            this.PowerShell
                .AddCommand("Remove-IdentityResource")
                    .AddParameter("Name", "name");

            this.PowerShell.Invoke();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();

            Assert.Empty(this.PowerShell.AddCommand("Get-IdentityResource").Invoke().ToArray());
        }

        [Fact]
        public void IdentityShell_removes_identity_from_pipe()
        {
            // ARRANGE

            var pso = ArrangeIdentityResource();

            // ACT

            this.PowerShell.AddCommand("Remove-IdentityResource");
            this.PowerShell.Invoke(new[] { pso });

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();

            Assert.Empty(this.PowerShell.AddCommand("Get-IdentityResource").Invoke().ToArray());
        }

        public static IEnumerable<object[]> OpenIdDefaults
        {
            get
            {
                object[] Objects(params object[] objs) => objs;

                yield return Objects(new IdentityResources.OpenId());
                yield return Objects(new IdentityResources.Email());
                yield return Objects(new IdentityResources.Address());
                yield return Objects(new IdentityResources.Phone());
                yield return Objects(new IdentityResources.Profile());
            }
        }

        [Theory]
        [MemberData(nameof(OpenIdDefaults))]
        public void IdentityShell_creates_identity_from_OpenId_default(IdentityResource r)
        {
            // ARRANGE
            // ACT

            this.PowerShell
                .AddCommand("Set-IdentityResource");

            this.PowerShell.Invoke(new object[] { r }).Single();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();
            var result = this.PowerShell.AddCommand("Get-IdentityResource").Invoke().Single();

            Assert.Equal(r.Description, result.Property<string>("Description"));
            Assert.Equal(r.DisplayName, result.Property<string>("DisplayName"));
            Assert.Equal(r.Emphasize, result.Property<bool>("Emphasize"));
            Assert.Equal(r.Enabled, result.Property<bool>("Enabled"));
            Assert.Equal(r.Name, result.Property<string>("Name"));
            Assert.Equal(r.Properties, result.Property<IDictionary<string, string>>("Properties"));
            Assert.Equal(r.Required, result.Property<bool>("Required"));
            Assert.Equal(r.ShowInDiscoveryDocument, result.Property<bool>("ShowInDiscoveryDocument"));
            Assert.Equal(r.UserClaims, result.Property<ICollection<string>>("UserClaims"));
        }
    }
}