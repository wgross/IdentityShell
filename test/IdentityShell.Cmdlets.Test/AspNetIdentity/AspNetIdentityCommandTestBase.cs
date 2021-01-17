using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;
using IdentityShell.Cmdlets.AspNetIdentity;
using IdentityShell.Cmdlets.Common;
using IdentityShell.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using Xunit;

namespace IdentityShell.Cmdlets.Test.AspNetIdentity
{
    [Collection(nameof(IdentityCommandBase.GlobalServiceProvider))]
    public abstract class AspNetIdentityCommandTestBase
    {
        private readonly ServiceProvider serviceProvider;
        private InMemoryDbContextOptionsBuilder inMemorySqliteDb;

        public AspNetIdentityCommandTestBase()
        {
            this.inMemorySqliteDb = new InMemoryDbContextOptionsBuilder($"{nameof(ApplicationDbContext)}-{Guid.NewGuid()}");

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddLogging(builder => builder.AddDebug());

            serviceCollection
                .AddDbContext<ApplicationDbContext>(opts =>
                {
                    this.inMemorySqliteDb.CreateOptions(opts,
                        sqlitsopts => sqlitsopts.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name));
                });

            serviceCollection
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            this.serviceProvider = serviceCollection.BuildServiceProvider();

            using var serviceScope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.EnsureCreated();

            IdentityCommandBase.GlobalServiceProvider = this.serviceProvider;
            this.PowerShell = PowerShell.Create(InitialSessionState.CreateDefault().AddAspIdentityCommands().AddCommonCommands());
        }

        protected UserManager<ApplicationUser> UserManager => this.serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

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