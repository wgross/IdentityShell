using IdentityServer4.Models;
using IdentityShell.Cmdlets;
using IdentityShell.Cmdlets.Configuration;
using System;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Xunit;

namespace IdentityShell.Cmdlets.Test
{
    public class NewIdentitySecretTest
    {
        public PowerShell PowerShell { get; }

        public NewIdentitySecretTest()
        {
            this.PowerShell = PowerShell.Create(InitialSessionState.CreateDefault().AddIdentityConfigurationCommands());
        }

        [Fact]
        public void NewIdentitySecret_create_Secret_instance()
        {
            // ACT

            var expiration = DateTime.Now;
            this.PowerShell
                .AddCommand<NewIdentitySecretCommand>()
                    .AddParameter(c => c.Value, "value")
                    .AddParameter(c => c.Description, "description")
                    .AddParameter(c => c.Expiration, expiration)
                    .End();

            var result = this.PowerShell.Invoke().Single();

            // ASSERT

            Assert.IsType<Secret>(result.ImmediateBaseObject);

            Assert.Equal("value", result.Property<string>("Value"));
            Assert.Equal("description", result.Property<string>("Description"));
            Assert.Equal(expiration, result.Property<DateTime>("Expiration"));
        }
    }
}