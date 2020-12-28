using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using IdentityShell.Cmdlets;
using IdentityShell.Cmdlets.Configuration;
using IdentityShell.Cmdlets.Test;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using Xunit;

namespace IdentityShell.Cmdlets.Test
{
    public abstract class IdentityConfigurationCommandTestBase : IDisposable
    {
        private readonly string connectionString;
        private readonly ServiceProvider serviceProvider;

        public IdentityConfigurationCommandTestBase()
        {
            string createConnectionString(Guid instanceId, string path) => $@"Data Source={path}\IdentityShell.Configuration.{instanceId}.db";

            this.connectionString = createConnectionString(
                instanceId: Guid.NewGuid(),
                path: Path.GetDirectoryName(typeof(IdentityClientCommandTest).GetTypeInfo().Assembly.Location));

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddIdentityServer()
                .AddConfigurationStore(options =>
                {
                    string migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

                    options.ConfigureDbContext = builder => builder.UseSqlite(this.connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                });

            this.serviceProvider = serviceCollection.BuildServiceProvider();

            using var serviceScope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            context.Database.EnsureCreated();

            IdentityCommandBase.GlobalServiceProvider = this.serviceProvider;
            this.PowerShell = PowerShell.Create(InitialSessionState.CreateDefault().AddIdentityConfigurationCommands());
        }

        public void Dispose()
        {
            this.serviceProvider
                .CreateScope()
                    .ServiceProvider
                    .GetRequiredService<ConfigurationDbContext>()
                        .Database
                        .EnsureDeleted();
        }

        public PowerShell PowerShell { get; }

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

            var pso = this.PowerShell.Invoke().Single();
            this.PowerShell.Commands.Clear();
            return pso;
        }
    }
}