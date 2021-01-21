using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using IdentityServerAspNetIdentity.Data;
using IdentityShell.AspNetIdentityStore.Data.Migrations;
using IdentityShell.Hosting;
using IdentityShell.IdentityStore.Data.Migrations.IdentityServer.ConfigurationDb;
using IdentityShell.IdentityStore.Data.Migrations.IdentityServer.PersistedGrantDb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace IdentityShell.IntegTest.TestServer
{
    public class TestServerStartup : Startup
    {
        public TestServerStartup(IWebHostEnvironment environment, IConfiguration configuration)
            : base(environment, configuration)
        { }

        public InMemoryDbContextOptionsBuilder AspNetIdentityStore { get; private set; }
        public InMemoryDbContextOptionsBuilder IdentityServerConfigurationStore { get; private set; }
        public InMemoryDbContextOptionsBuilder IdentityServerOperationalStore { get; private set; }

        protected override void ConfigureAspNetIdentityDbContext(DbContextOptionsBuilder options)
        {
            this.AspNetIdentityStore ??= new IdentityShell.Hosting.InMemoryDbContextOptionsBuilder($"{nameof(ApplicationDbContext)}-{Guid.NewGuid()}");

            this.AspNetIdentityStore.CreateOptions(options,
                sqlitsopts => sqlitsopts.MigrationsAssembly(typeof(CreateIdentitySchema).GetTypeInfo().Assembly.GetName().Name));
        }

        protected override void ConfigureIdentityServerConfigurationStore(ConfigurationStoreOptions options)
        {
            this.IdentityServerConfigurationStore ??= new InMemoryDbContextOptionsBuilder($"{nameof(ConfigurationDbContext)}-{Guid.NewGuid()}");

            options.ConfigureDbContext = builder => this.IdentityServerConfigurationStore.CreateOptions(builder,
                sqliteOpts => sqliteOpts.MigrationsAssembly(typeof(InitialIdentityServerConfigurationDbMigration).GetTypeInfo().Assembly.GetName().Name));
        }

        protected override void ConfigureIdentityServerOperationalStore(OperationalStoreOptions options)
        {
            this.IdentityServerOperationalStore ??= new InMemoryDbContextOptionsBuilder($"{nameof(PersistedGrantDbContext)}-{Guid.NewGuid()}");

            options.ConfigureDbContext = builder => this.IdentityServerOperationalStore.CreateOptions(builder,
                sqliteOpts => sqliteOpts.MigrationsAssembly(typeof(InitialIdentityServerPersistedGrantDbMigration).GetTypeInfo().Assembly.GetName().Name));
        }

        protected override void OnShuttingDown()
        {
            // singletons added as a specific instance won't be disposed by the framework
            // therefore dispose manually and hold sigleton as a startup class member

            this.AspNetIdentityStore?.Dispose();
            this.AspNetIdentityStore = null;

            this.IdentityServerConfigurationStore?.Dispose();
            this.IdentityServerConfigurationStore = null;

            this.IdentityServerOperationalStore?.Dispose();
            this.IdentityServerOperationalStore = null;

            base.OnShuttingDown();
        }
    }
}