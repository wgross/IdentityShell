using Duende.IdentityServer.Models;
using IdentityShell.Commands.Configuration;
using IdentityShell.Commands.Configuration.ArgumentCompleters;
using System.Collections;
using System.Linq;
using System.Management.Automation;
using Xunit;

namespace IdentityShell.Commands.Test.Configuration
{
    [Collection(nameof(IdentityCommandBase.GlobalServiceProvider))]
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
            this.AssertApiScope(result);
        }

        [Fact]
        public void IdentityShell_reads_all_ApiScope()
        {
            // ARRANGE
            PSObject pso = this.ArrangeApiScope();

            // ACT
            this.PowerShell.AddCommandEx<GetIdentityApiScopeCommand>();

            var result = this.PowerShell.Invoke().Single();

            // ASSERT
            this.AssertApiScope(result);
        }

        [Fact]
        public void IdentityShell_reads_ApiScope_by_name()
        {
            // ARRANGE
            PSObject pso = this.ArrangeApiScope();

            // ACT
            this.PowerShell.AddCommandEx<GetIdentityApiScopeCommand>(cmd => cmd.AddParameter(c => c.Name, "name"));

            var result = this.PowerShell.Invoke().Single();

            // ASSERT
            this.AssertApiScope(result);
        }

        [Fact]
        public void IdentityShell_modifies_pipes_ApiScope()
        {
            // ARRANGE
            PSObject pso = this.ArrangeApiScope();

            // ACT
            var result = this.PowerShell
                .AddCommandEx<SetIdentityApiScopeCommand>(cmd => cmd.AddParameter(c => c.Description, "description-changed"))
                .Invoke(Array(pso))
                .Single();

            // ASSERT
            Assert.False(this.PowerShell.HadErrors);
            Assert.Equal("description-changed", result.As<ApiScope>().Description);
        }

        [Fact]
        public void IdentityShell_deletes_ApiScope()
        {
            // ARRANGE
            PSObject pso = this.ArrangeApiScope();

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

        [Fact]
        public void ApiScopeNameCompleter_completes_prefix()
        {
            // ARRANGE
            this.ArrangeApiScope();

            // ACT
            var completer = new IdentityApiScopeNameCompleter();

            var result = completer.CompleteArgument(commandName: null, parameterName: null, "na", commandAst: null, fakeBoundParameters: null);

            // ASSERT
            Assert.Equal("name", result.Single().CompletionText);
        }
    }
}