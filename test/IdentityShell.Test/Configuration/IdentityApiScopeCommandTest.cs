using IdentityShell.Cmdlets.Configuration;
using System.Collections;
using System.Linq;
using Xunit;

namespace IdentityShell.Test.Configuration
{
    public class IdentityApiScopeCommandTest : IdentityConfigurationCommandTestBase
    {
        [Fact]
        public void IdentityShell_creates_ApiScope()
        {
            // ACT

            this.PowerShell
                .AddCommandEx<SetIdentityApiScopeCommand>(cmd =>
                {
                    cmd
                        .AddParameter(c => c.Name, "name")
                        .AddParameter(c => c.DisplayName, "displayName")
                        .AddParameter(c => c.Description, "description")
                        .AddParameter(c => c.Enabled, true)
                        .AddParameter(c => c.ShowInDiscoveryDocument, true)
                        .AddParameter(c => c.Required, true)
                        .AddParameter(c => c.Properties, new Hashtable { { "p1", "v1" } })
                        .AddParameter(c => c.UserClaims, new[] { "claim" });
                });

            var result = this.PowerShell.Invoke().Single();

            // ASSERT

            AssertApiScope(result);
        }

        [Fact]
        public void IdentityShell_reads_all_ApiScope()
        {
            // ARRANGE

            var pso = this.ArrangeApiScope();

            // ACT

            this.PowerShell.AddCommandEx<GetIdentityApiScopeCommand>();

            var result = this.PowerShell.Invoke().Single();

            AssertApiScope(result);
        }

        [Fact]
        public void IdentityShell_reads_ApiScope_by_name()
        {
            // ARRANGE

            var pso = this.ArrangeApiScope();

            // ACT

            this.PowerShell.AddCommandEx<GetIdentityApiScopeCommand>(cmd => cmd.AddParameter(c => c.Name, "name"));

            var result = this.PowerShell.Invoke().Single();

            AssertApiScope(result);
        }

        [Fact]
        public void IdentityShell_deletes_ApiScope()
        {
            // ARRANGE

            var pso = this.ArrangeApiScope();

            // ACT

            this.PowerShell.AddCommandEx<RemoveIdentityApiScopeCommand>();

            var result = this.PowerShell.Invoke(Array(pso)).ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Empty(result);

            this.PowerShell.Commands.Clear();
            var readApiScopes = this.PowerShell.AddCommandEx<GetIdentityApiScopeCommand>().Invoke().ToArray();

            Assert.Empty(readApiScopes);
        }
    }
}