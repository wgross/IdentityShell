using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;
using IdentityShell.Cmdlets;
using IdentityShell.Cmdlets.AspNetIdentity;
using IdentityShell.Cmdlets.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using Xunit;

namespace IdentityShell.Cmdlets.Test.AspNetIdentity
{
    [Collection(nameof(IdentityCommandBase.GlobalServiceProvider))]
    public abstract class AspNetIdentityCommandTestBase
    {
        private readonly string connectionString;
        private readonly ServiceProvider serviceProvider;

        public AspNetIdentityCommandTestBase()
        {
            string createConnectionString(Guid instanceId, string path) => $@"Data Source={path}\IdentityShell.AspNetIdentity.{instanceId}.db";

            this.connectionString = createConnectionString(
                instanceId: Guid.NewGuid(),
                path: Path.GetDirectoryName(typeof(AspNetIdentityCommandTestBase).GetTypeInfo().Assembly.Location));

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddLogging(builder => builder.AddDebug());

            serviceCollection
              .AddDbContext<ApplicationDbContext>(options =>
              {
                  string migrationAssembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name;

                  options.UseSqlite(this.connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
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
            this.serviceProvider
                .CreateScope()
                    .ServiceProvider
                    .GetRequiredService<ApplicationDbContext>()
                        .Database
                        .EnsureDeleted();
        }

        public PowerShell PowerShell { get; }

        protected static object[] Array(params object[] items) => items;
    }
}