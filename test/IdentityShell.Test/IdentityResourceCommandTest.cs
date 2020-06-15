using IdentityServer4.EntityFramework.DbContexts;
using IdentityShell.Commands;
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

namespace IdentityShell.Test
{
    public class IdentityResourceCommandTest
    {
        private readonly IServiceProvider serviceProvider;
        private readonly string connectionString;

        public PowerShell PowerShell { get; }

        public IdentityResourceCommandTest()
        {
            string createConnectionString(Guid instanceId, string path) => $@"Data Source=(LocalDb)\MSSQLLocalDB;database=IdentityShell.Test.{instanceId};trusted_connection=yes;AttachDBFilename={path}\IdentityShell-{instanceId}.mdf";

            this.connectionString = createConnectionString(
                instanceId: Guid.NewGuid(),
                path: Path.GetDirectoryName(typeof(IdentityClientCommandTest).GetTypeInfo().Assembly.Location));

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddIdentityServer()
                .AddConfigurationStore(options =>
                {
                    string migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

                    options.ConfigureDbContext = builder => builder.UseSqlServer(this.connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                });

            this.serviceProvider = serviceCollection.BuildServiceProvider();

            using var serviceScope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            context.Database.EnsureCreated();

            IdentityCommandBase.ServiceProvider = this.serviceProvider;
            this.PowerShell = PowerShell.Create(InitialSessionState.CreateDefault().AddIdentityCommands());
        }

        private PSObject ArrangeIdentityResource()
        {
            this.PowerShell
                   .AddCommand("Set-IdentityResource")
                       .AddParameter("Name", "name")
                       .AddParameter("DisplayName", "displayName")
                       .AddParameter("Description", "description")
                       .AddParameter("ShowInDiscoveryDocument", true)
                       .AddParameter("UserClaims", new[] { "claim-1", "claim-2" })
                       .AddParameter("Properties", new Hashtable
                       {
                        {"p1", "v1" },
                        {"p2", "v2" }
                       })
                       .AddParameter("Required", true)
                       .AddParameter("Emphasize", true);

            var pso = this.PowerShell.Invoke().Single();
            this.PowerShell.Commands.Clear();
            return pso;
        }

        [Fact]
        public void IdentityShell_read_empty_user_table()
        {
            // ACT

            this.PowerShell
                .AddCommand("Get-IdentityResource");

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Empty(result);
        }

        [Fact]
        public void IdentityShell_creates_identity()
        {
            // ACT

            this.PowerShell
                .AddCommand("Set-IdentityResource")
                    .AddParameter("Name", "name")
                    .AddParameter("DisplayName", "displayName")
                    .AddParameter("Description", "description")
                    .AddParameter("ShowInDiscoveryDocument", true)
                    .AddParameter("UserClaims", new[] { "claim-1", "claim-2" })
                    .AddParameter("Properties", new Hashtable
                    {
                        {"p1", "v1" },
                        {"p2", "v2" }
                    })
                    .AddParameter("Required", true)
                    .AddParameter("Emphasize", true);

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Single(result);
        }

        [Fact]
        public void IdentityShell_reads_all_identities()
        {
            // ARRANGE

            var ir = ArrangeIdentityResource();

            // ACT

            this.PowerShell.AddCommand("Get-IdentityResource");
            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.Single();

            Assert.True(resultValue.Property<bool>("Enabled"));
            Assert.Equal("name", resultValue.Property<string>("Name"));
            Assert.Equal("displayName", resultValue.Property<string>("DisplayName"));
            Assert.Equal("description", resultValue.Property<string>("Description"));
            Assert.True(resultValue.Property<bool>("Required"));
            Assert.True(resultValue.Property<bool>("Emphasize"));
            Assert.True(resultValue.Property<bool>("ShowInDiscoveryDocument"));
            Assert.Equal(new[] { "claim-1", "claim-2" }, resultValue.Property<ICollection<string>>("UserClaims"));
            Assert.Equal(new Dictionary<string, string>
            {
                {"p1", "v1" },
                {"p2", "v2" }
            },
            resultValue.Property<IDictionary<string, string>>("Properties"));
        }

        [Fact]
        public void IdentityShell_modifies_piped_identity()
        {
            var pso = ArrangeIdentityResource();

            // ACT

            this.PowerShell
                .AddCommand("Set-IdentityResource")
                    .AddParameter("Required", false);

            var result = this.PowerShell.Invoke(new[] { pso }).ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.Single();

            Assert.False(resultValue.Property<bool>("Required"));
            Assert.Same(pso.ImmediateBaseObject, resultValue.ImmediateBaseObject);
        }
    }
}