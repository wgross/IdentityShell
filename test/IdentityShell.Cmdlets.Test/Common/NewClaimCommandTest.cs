using IdentityModel;
using IdentityShell.Cmdlets;
using IdentityShell.Cmdlets.Common;
using IdentityShell.Cmdlets.Configuration;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Claims;
using Xunit;

namespace IdentityShell.Cmdlets.Test
{
    public class NewClaimCommandTest
    {
        public PowerShell PowerShell { get; }

        public NewClaimCommandTest()
        {
            this.PowerShell = PowerShell.Create(InitialSessionState.CreateDefault().AddCommonCommands());
            this.PowerShell.Commands.Clear();
        }

        [Fact]
        public void NewIdentityClaimCommand_creates_new_Claim()
        {
            // ACT

            this.PowerShell
                .AddCommand<NewClaimCommand>()
                    .AddParameter(c => c.Type, "type")
                    .AddParameter(c => c.Value, "value")
                    .AddParameter(c => c.ValueType, "valueType")
                    .End();

            var result = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.IsType<Claim>(result.ImmediateBaseObject);

            Assert.Equal("type", result.Property<string>("Type"));
            Assert.Equal("value", result.Property<string>("Value"));
            Assert.Equal("valueType", result.Property<string>("ValueType"));
        }

        [Fact]
        public void NewIdentityClaimCommand_creates_new_Claim_from_symbolic_claim_name()
        {
            // ACT

            this.PowerShell
                .AddCommand<NewClaimCommand>()
                    .AddParameter(c => c.Type, nameof(JwtClaimTypes.Name))
                    .AddParameter(c => c.Value, "value")
                    .AddParameter(c => c.ValueType, nameof(ClaimValueTypes.String))
                    .End();

            var result = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.IsType<Claim>(result.ImmediateBaseObject);
            Assert.Equal(JwtClaimTypes.Name, result.Property<string>("Type"));
            Assert.Equal("value", result.Property<string>("Value"));
            Assert.Equal(ClaimValueTypes.String, result.Property<string>("ValueType"));
        }
    }
}