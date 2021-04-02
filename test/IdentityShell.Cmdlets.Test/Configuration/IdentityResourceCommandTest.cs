using Duende.IdentityServer.Models;
using IdentityShell.Commands.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Xunit;

namespace IdentityShell.Commands.Test.Configuration
{
    [Collection(nameof(IdentityCommandBase.GlobalServiceProvider))]
    public class IdentityResourceCommandTest : IdentityConfigurationCommandTestBase
    {
        private PSObject ArrangeIdentityResource()
        {
            this.PowerShell
                   .AddCommandEx<SetIdentityResourceCommand>(cmd =>
                   {
                       cmd
                       .AddParameter(c => c.Name, "name")
                       .AddParameter(c => c.DisplayName, "displayName")
                       .AddParameter(c => c.Description, "description")
                       .AddParameter(c => c.ShowInDiscoveryDocument, true)
                       .AddParameter(c => c.UserClaims, Array("claim-1", "claim-2"))
                       .AddParameter(c => c.Properties, new Hashtable
                       {
                        {"p1", "v1" },
                        {"p2", "v2" }
                       })
                       .AddParameter(c => c.Required, true)
                       .AddParameter(c => c.Emphasize, true);
                   });

            var pso = this.PowerShell.Invoke().Single();
            this.PowerShell.Commands.Clear();
            return pso;
        }

        [Fact]
        public void IdentityShell_read_empty_user_table()
        {
            // ACT
            this.PowerShell
                .AddCommandEx<GetIdentityResourceCommand>();

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
                .AddCommandEx<SetIdentityResourceCommand>(cmd =>
                {
                    cmd
                    .AddParameter(c => c.Name, "name")
                    .AddParameter(c => c.DisplayName, "displayName")
                    .AddParameter(c => c.Description, "description")
                    .AddParameter(c => c.ShowInDiscoveryDocument, true)
                    .AddParameter(c => c.UserClaims, Array("claim-1", "claim-2"))
                    .AddParameter(c => c.Properties, new Hashtable
                    {
                        {"p1", "v1" },
                        {"p2", "v2" }
                    })
                    .AddParameter(c => c.Required, true)
                    .AddParameter(c => c.Emphasize, true);
                });

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT
            Assert.False(this.PowerShell.HadErrors);
            Assert.Single(result);
        }

        [Fact]
        public void IdentityShell_reads_all_identities()
        {
            // ARRANGE
            var pso = this.ArrangeIdentityResource();

            // ACT
            this.PowerShell.AddCommandEx<GetIdentityResourceCommand>();
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
            // ARRANGE
            var pso = this.ArrangeIdentityResource();

            // ACT
            this.PowerShell
                .AddCommandEx<SetIdentityResourceCommand>(cmdlet => cmdlet.AddParameter(c => c.Required, false));

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
            var pso = this.ArrangeIdentityResource();

            // ACT
            this.PowerShell
                .AddCommandEx<RemoveIdentityResourceCommand>(cmd => cmd.AddParameter(c => c.Name, "name"));

            this.PowerShell.Invoke();

            // ASSERT
            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();

            Assert.Empty(this.PowerShell.AddCommandEx<GetIdentityResourceCommand>().Invoke().ToArray());
        }

        [Fact]
        public void IdentityShell_removes_identity_from_pipe()
        {
            // ARRANGE
            var pso = this.ArrangeIdentityResource();

            // ACT
            this.PowerShell.AddCommandEx<RemoveIdentityResourceCommand>();
            this.PowerShell.Invoke(new[] { pso });

            // ASSERT
            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();

            Assert.Empty(this.PowerShell.AddCommandEx<GetIdentityResourceCommand>().Invoke().ToArray());
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
            // ACT
            this.PowerShell
                .AddCommandEx<SetIdentityResourceCommand>();

            this.PowerShell.Invoke(new object[] { r }).Single();

            // ASSERT
            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();
            var result = this.PowerShell.AddCommandEx<GetIdentityResourceCommand>().Invoke().Single();

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