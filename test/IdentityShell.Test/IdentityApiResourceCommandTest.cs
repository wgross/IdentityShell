using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
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
    [Collection(nameof(ConfigurationDbContext))]
    public class IdentityApiResourceCommandTest
    {
        private readonly IServiceProvider serviceProvider;
        private readonly string connectionString;

        public PowerShell PowerShell { get; }

        public IdentityApiResourceCommandTest()
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

        private PSObject ArrangeIdentityApiResource(DateTime secretExpiration)
        {
            this.PowerShell
                   .AddCommand("Set-IdentityApiResource")
                        .AddParameter("Name", "name")
                        .AddParameter("DisplayName", "displayName")
                        .AddParameter("Description", "description")
                        .AddParameter("UserClaims", new[] { "claim-1", "claim-2" })
                        .AddParameter("Properties", new Hashtable
                        {
                        {"p1", "v1" },
                        {"p2", "v2" }
                        })
                        .AddParameter("ApiSecrets", new[]
                        {
                            new Secret("value", "description", secretExpiration)
                        })
                        .AddParameter("Scopes", new[]
                        {
                            new Scope("name", "displayName", new[]{"claimType" })
                            {
                                Description = "description",
                                Emphasize = true,
                                Required = true,
                                ShowInDiscoveryDocument = true
                            }
                        });

            var pso = this.PowerShell.Invoke().Single();
            this.PowerShell.Commands.Clear();
            return pso;
        }

        [Fact]
        public void IdentityShell_reads_empty_api_table()
        {
            // ACT

            this.PowerShell
                .AddCommand("Get-IdentityApiResource");

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Empty(result);
        }

        [Fact]
        public void IdentityShell_creates_api()
        {
            // ACT
            var secretExpiration = DateTime.Now;

            this.PowerShell
                .AddCommand("Set-IdentityApiResource")
                    .AddParameter("Name", "name")
                    .AddParameter("DisplayName", "displayName")
                    .AddParameter("Description", "description")
                    .AddParameter("UserClaims", new[] { "claim-1", "claim-2" })
                    .AddParameter("Properties", new Hashtable
                    {
                    {"p1", "v1" },
                    {"p2", "v2" }
                    })
                    .AddParameter("ApiSecrets", new[]
                    {
                        new Secret("value", "description", secretExpiration)
                    })
                    .AddParameter("Scopes", new[]
                    {
                        new Scope("name", "displayName", new[]{"claimType" })
                        {
                            Description = "description",
                            Emphasize = true,
                            Required = true,
                            ShowInDiscoveryDocument = true
                        }
                    });

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);
            Assert.Single(result);
        }

        [Fact]
        public void IdentityShell_reads_all_apis()
        {
            // ARRANGE
            var secretExpiration = DateTime.Now;
            var pso = ArrangeIdentityApiResource(secretExpiration);

            // ACT

            this.PowerShell.AddCommand("Get-IdentityApiResource");
            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.Single();

            Assert.True(resultValue.Property<bool>("Enabled"));
            Assert.Equal("name", resultValue.Property<string>("Name"));
            Assert.Equal("displayName", resultValue.Property<string>("DisplayName"));
            Assert.Equal("description", resultValue.Property<string>("Description"));
            Assert.Equal(new[] { "claim-1", "claim-2" }, resultValue.Property<ICollection<string>>("UserClaims"));
            Assert.Equal(new Dictionary<string, string>
            {
                {"p1", "v1" },
                {"p2", "v2" }
            },
            resultValue.Property<IDictionary<string, string>>("Properties"));

            Assert.Equal("name", resultValue.Property<ICollection<Scope>>("Scopes").Single().Name);
            Assert.Equal("description", resultValue.Property<ICollection<Scope>>("Scopes").Single().Description);
            Assert.Equal("displayName", resultValue.Property<ICollection<Scope>>("Scopes").Single().DisplayName);
            Assert.True(resultValue.Property<ICollection<Scope>>("Scopes").Single().Emphasize);
            Assert.True(resultValue.Property<ICollection<Scope>>("Scopes").Single().ShowInDiscoveryDocument);
            Assert.True(resultValue.Property<ICollection<Scope>>("Scopes").Single().Required);
        }

        [Fact]
        public void IdentityShell_modifies_piped_api()
        {
            var secretExpiration = DateTime.Now;
            var pso = ArrangeIdentityApiResource(secretExpiration);

            // ACT

            this.PowerShell
                .AddCommand("Get-IdentityApiResource")
                .AddCommand("Set-IdentityApiResource")
                    .AddParameter("DisplayName", "displayname-changed");

            var result = this.PowerShell.Invoke().ToArray();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            var resultValue = result.Single();

            Assert.Equal("displayname-changed", resultValue.Property<string>("DisplayName"));
        }

        [Fact]
        public void IdentityShell_removes_api()
        {
            // ARRANGE

            var secretExpiration = DateTime.Now;
            var pso = ArrangeIdentityApiResource(secretExpiration);

            // ACT

            this.PowerShell
                .AddCommand("Remove-IdentityApiResource")
                    .AddParameter("Name", "name");

            this.PowerShell.Invoke();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();

            Assert.Empty(this.PowerShell.AddCommand("Get-IdentityApiResource").Invoke().ToArray());
        }

        [Fact]
        public void IdentityShell_removes_api_from_pipe()
        {
            // ARRANGE

            var secretExpiration = DateTime.Now;
            ArrangeIdentityApiResource(secretExpiration);

            // ACT

            this.PowerShell
                .AddCommand("Get-IdentityApiResource")
                .AddCommand("Remove-IdentityApiResource");
            this.PowerShell.Invoke();

            // ASSERT

            Assert.False(this.PowerShell.HadErrors);

            this.PowerShell.Commands.Clear();

            Assert.Empty(this.PowerShell.AddCommand("Get-IdentityApiResource").Invoke().ToArray());
        }
    }
}