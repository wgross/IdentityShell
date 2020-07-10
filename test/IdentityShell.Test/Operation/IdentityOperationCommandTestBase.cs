using IdentityServer4.EntityFramework.DbContexts;
using IdentityShell.Cmdlets;
using IdentityShell.Cmdlets.Configuration;
using IdentityShell.Cmdlets.Operation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;

namespace IdentityShell.Test.Operation
{
    public class IdentityOperationCommandTestBase
    {
        private readonly string connectionString;
        protected readonly ServiceProvider serviceProvider;

        public IdentityOperationCommandTestBase()
        {
            string createConnectionString(Guid instanceId, string path) => $@"Data Source={path}\IdentityShell.Operation.{instanceId}.db";

            this.connectionString = createConnectionString(
                instanceId: Guid.NewGuid(),
                path: Path.GetDirectoryName(typeof(IdentityClientCommandTest).GetTypeInfo().Assembly.Location));

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddIdentityServer()
                .AddOperationalStore(options =>
                {
                    string migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

                    options.ConfigureDbContext = builder => builder.UseSqlite(this.connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                });

            this.serviceProvider = serviceCollection.BuildServiceProvider();

            using var serviceScope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

            context.Database.EnsureCreated();

            IdentityCommandBase.GlobalServiceProvider = this.serviceProvider;
            this.PowerShell = PowerShell.Create(InitialSessionState.CreateDefault().AddIdentityOperationCommands());
        }

        public void Dispose()
        {
            this.serviceProvider
                .CreateScope()
                    .ServiceProvider
                    .GetRequiredService<PersistedGrantDbContext>()
                        .Database
                        .EnsureDeleted();
        }

        public PowerShell PowerShell { get; }

        protected static object[] Array(params object[] items) => items;
    }
}