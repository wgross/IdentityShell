using Duende.IdentityServer.Models;
using IdentityShell.Commands.Configuration;
using IdentityShell.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Xunit;

namespace IdentityShell.Commands.Test
{
    public abstract class IdentityConfigurationCommandTestBase : IDisposable
    {
        public IdentityConfigurationCommandTestBase()
        {
            IdentityCommandBase.GlobalServiceProvider = new ServiceCollection()
                .AddSingleton<IdentityServerInMemoryConfig>()
                .AddScoped<IApiScopeRepository, ApiScopeRepository>()
                .AddScoped<IApiResourceRepository, ApiResourceRepository>()
                .AddScoped<IClientRepository, ClientRepository>()
                .AddScoped<IIdentityResourceRepository, IdentityResourceRepository>()
                .AddScoped<ITestUserRepository, TestUserRepository>()
                .BuildServiceProvider();

            this.PowerShell = PowerShell.Create(InitialSessionState.CreateDefault().AddIdentityConfigurationCommands());
        }

        public void Dispose()
        {
            this.PowerShell?.Dispose();
            this.PowerShell = null;
        }

        public PowerShell PowerShell { get; private set; }

        protected static object[] Array(params object[] items) => items;

        protected void AssertApiScope(PSObject result)
        {
            Assert.False(this.PowerShell.HadErrors);
            Assert.IsType<ApiScope>(result.ImmediateBaseObject);
            Assert.Equal("name", result.Property<string>("Name"));
            Assert.Equal("displayName", result.Property<string>("DisplayName"));
            Assert.Equal("description", result.Property<string>("Description"));
            Assert.True(result.Property<bool>("Enabled"));
            Assert.True(result.Property<bool>("Required"));
            Assert.True(result.Property<bool>("ShowInDiscoveryDocument"));
            Assert.Equal(new Dictionary<string, string>() { { "p1", "v1" } }, result.Property<IDictionary<string, string>>("Properties"));
            Assert.Equal("claim", result.Property<ICollection<string>>("UserClaims").Single());
        }

        protected PSObject ArrangeApiScope()
        {
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

            PSObject pso = this.PowerShell.Invoke().Single();
            this.PowerShell.Commands.Clear();
            return pso;
        }
    }
}