using IdentityServer4.Models;
using IdentityShell.Cmdlets;
using IdentityShell.Cmdlets.Common;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Xunit;

namespace IdentityShell.Test
{
    public class NewIdentityClientClaimCommandTest
    {
        public PowerShell PowerShell { get; }

        public NewIdentityClientClaimCommandTest()
        {
            this.PowerShell = PowerShell.Create(InitialSessionState.CreateDefault().AddCommonCommands());
            this.PowerShell.Commands.Clear();
        }

        [Fact]
        public void IdentityShell_creates_ClientClaim()
        {
            // ACT

            this.PowerShell
                .AddCommandEx<NewIdentityClientClaimCommand>(cmdlet =>
                {
                    cmdlet
                        .AddParameter(c => c.Type, "type")
                        .AddParameter(c => c.Value, "value")
                        .AddParameter(c => c.ValueType, "valueType");
                });

            var result = this.PowerShell.Invoke().Single();

            // ASSERT

            var resultValue = result.As<ClientClaim>();

            Assert.Equal("type", resultValue.Type);
            Assert.Equal("value", resultValue.Value);
            Assert.Equal("valueType", resultValue.ValueType);
        }
    }
}