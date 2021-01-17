using IdentityServer4.EntityFramework.DbContexts;
using IdentityShell.Cmdlets.Operation;
using IdentityShell.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;

namespace IdentityShell.Cmdlets.Test.Operation
{
    public class IdentityOperationCommandTestBase
    {
        protected readonly ServiceProvider serviceProvider;
        private InMemoryDbContextOptionsBuilder inMemorySqliteDb;

        public IdentityOperationCommandTestBase()
        {
            this.inMemorySqliteDb = new InMemoryDbContextOptionsBuilder(nameof(PersistedGrantDbContext));

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddIdentityServer()
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = opts => this.inMemorySqliteDb.CreateOptions(opts,
                        sqliteOpts => sqliteOpts.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));
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
            this.PowerShell?.Dispose();
            this.PowerShell = null;

            this.inMemorySqliteDb?.Dispose();
            this.inMemorySqliteDb = null;
        }

        public PowerShell PowerShell { get; private set; }

        protected static object[] Array(params object[] items) => items;
    }
}