using IdentityServer4.EntityFramework.DbContexts;
using IdentityShell.Cmdlets;
using IdentityShell.Cmdlets.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;

namespace IdentityShell.Test
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
    }
}