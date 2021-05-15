using IdentityModel;
using IdentityShell.Commands.Common;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Claims;
using Xunit;

namespace IdentityShell.Commands.Test
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
                .AddCommandEx<NewClaimCommand>(cmd =>
                {
                    cmd.AddParameter(c => c.Type, "type");
                    cmd.AddParameter(c => c.Value, "value");
                    cmd.AddParameter(c => c.ValueType, "valueType");
                    cmd.AddParameter(c => c.Issuer, "issuer");
                    cmd.AddParameter(c => c.OriginalIssuer, "originalissuer");
                });

            var result = this.PowerShell.Invoke().Single();

            // ASSERT
            Assert.IsType<Claim>(result.ImmediateBaseObject);
            Assert.Equal("type", result.Property<string>("Type"));
            Assert.Equal("value", result.Property<string>("Value"));
            Assert.Equal("valueType", result.Property<string>("ValueType"));
            Assert.Equal("issuer", result.Property<string>("Issuer"));
            Assert.Equal("originalissuer", result.Property<string>("OriginalIssuer"));
        }

        [Fact]
        public void NewIdentityClaimCommand_creates_new_Claim_from_symbolic_claim_name()
        {
            // ACT
            this.PowerShell
                .AddCommandEx<NewClaimCommand>(cmd =>
                {
                    cmd.AddParameter(c => c.Type, nameof(JwtClaimTypes.Name));
                    cmd.AddParameter(c => c.Value, "value");
                    cmd.AddParameter(c => c.ValueType, nameof(ClaimValueTypes.String));
                });

            var result = this.PowerShell.Invoke().Single();

            // ASSERT
            Assert.IsType<Claim>(result.ImmediateBaseObject);
            Assert.Equal(JwtClaimTypes.Name, result.Property<string>("Type"));
            Assert.Equal("value", result.Property<string>("Value"));
            Assert.Equal(ClaimValueTypes.String, result.Property<string>("ValueType"));
        }
    }
}