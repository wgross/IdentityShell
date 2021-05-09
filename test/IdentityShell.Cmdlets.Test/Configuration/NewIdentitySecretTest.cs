using Duende.IdentityServer.Models;
using IdentityShell.Commands.Configuration;
using System;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Xunit;

namespace IdentityShell.Commands.Test
{
    [Collection(nameof(IdentityCommandBase.GlobalServiceProvider))]
    public class NewIdentitySecretTest
    {
        public PowerShell PowerShell { get; }

        public NewIdentitySecretTest()
        {
            this.PowerShell = PowerShell.Create(InitialSessionState.CreateDefault().AddIdentityConfigurationCommands());
        }

        [Fact]
        public void NewIdentitySecret_create_secret_instance()
        {
            // ACT
            var expiration = DateTime.Now;
            this.PowerShell
                .AddCommandEx<NewIdentitySecretCommand>(cmd =>
                {
                    cmd
                    .AddParameter(c => c.Value, "value")
                    .AddParameter(c => c.Description, "description")
                    .AddParameter(c => c.Expiration, expiration);
                });

            var result = this.PowerShell.Invoke().Single();

            // ASSERT
            Assert.IsType<Secret>(result.ImmediateBaseObject);
            Assert.Equal("value", result.Property<string>("Value"));
            Assert.Equal("description", result.Property<string>("Description"));
            Assert.Equal(expiration, result.Property<DateTime>("Expiration"));
        }

        [Fact]
        public void NewIdentitySecret_create_secret_instance_from_plain_text()
        {
            // ACT
            var expiration = DateTime.Now;
            this.PowerShell
                .AddCommandEx<NewIdentitySecretCommand>(cmd =>
                {
                    cmd.AddParameter(c => c.PlainText, "value")
                       .AddParameter(c => c.Description, "description")
                       .AddParameter(c => c.Expiration, expiration);
                });

            var result = this.PowerShell.Invoke().Single();

            // ASSERT
            Assert.IsType<Secret>(result.ImmediateBaseObject);
            Assert.Equal("value".Sha256(), result.Property<string>("Value"));
            Assert.Equal("description", result.Property<string>("Description"));
            Assert.Equal(expiration, result.Property<DateTime>("Expiration"));
        }
    }
}