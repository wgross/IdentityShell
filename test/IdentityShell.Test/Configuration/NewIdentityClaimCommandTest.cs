using IdentityShell.Cmdlets;
using IdentityShell.Cmdlets.Configuration;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Claims;
using Xunit;

namespace IdentityShell.Test
{
    public class NewIdentityClaimCommandTest
    {
        public PowerShell PowerShell { get; }

        public NewIdentityClaimCommandTest()
        {
            this.PowerShell = PowerShell.Create(InitialSessionState.CreateDefault().AddIdentityConfigurationCommands());
            this.PowerShell.Commands.Clear();
        }

        [Fact]
        public void NewIdentityClaimCommand_creates_new_Claim()
        {
            // ACT

            this.PowerShell
                .AddCommand<NewIdentityClaimCommand>()
                    .AddParameter(c => c.Type, "type")
                    .AddParameter(c => c.Value, "value")
                    .AddParameter(c => c.ValueType, nameof(ClaimValueTypes.String))
                    .End();

            var result = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.IsType<Claim>(result.ImmediateBaseObject);

            Assert.Equal("type", result.Property<string>("Type"));
            Assert.Equal("value", result.Property<string>("Value"));
            Assert.Equal(ClaimValueTypes.String, result.Property<string>("ValueType"));
        }
    }
}