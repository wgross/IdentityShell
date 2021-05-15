using Duende.IdentityServer.Models;
using IdentityShell.Commands.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Xunit;

namespace IdentityShell.Commands.Test
{
    [Collection(nameof(IdentityCommandBase.GlobalServiceProvider))]
    public class NewIdentityScopeCommandTest
    {
        public PowerShell PowerShell { get; }

        public NewIdentityScopeCommandTest()
        {
            this.PowerShell = PowerShell.Create(InitialSessionState.CreateDefault().AddIdentityConfigurationCommands());
            this.PowerShell.Commands.Clear();
        }

        [Fact]
        public void NewIdentityClaimCommand_creates_new_Claim()
        {
            // ACT1
            this.PowerShell
                .AddCommandEx<NewIdentityScopeCommand>(cmd =>
                {
                    cmd
                    .AddParameter(c => c.Name, "name")
                    .AddParameter(c => c.DisplayName, "displayName")
                    .AddParameter(c => c.Description, "description")
                    .AddParameter(c => c.Emphasize, true)
                    .AddParameter(c => c.Required, true)
                    .AddParameter(c => c.ShowInDiscoveryDocument, true)
                    .AddParameter(c => c.UserClaims, new object[] { "a", "b" });
                });

            var result = this.PowerShell.Invoke().Single();

            // ASSERT
            Assert.IsType<ApiScope>(result.ImmediateBaseObject);
            Assert.Equal("name", result.Property<string>("Name"));
            Assert.Equal("displayName", result.Property<string>("displayName"));
            Assert.Equal("description", result.Property<string>("Description"));
            Assert.Equal(new[] { "a", "b" }, result.Property<ICollection<string>>("UserClaims"));
            Assert.True(result.Property<bool>("Emphasize"));
            Assert.True(result.Property<bool>("Required"));
            Assert.True(result.Property<bool>("ShowInDiscoveryDocument"));
        }
    }
}