using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace IdentityShell.Test
{
    public class GetIdentityClientCommandTest : IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private readonly string connectionString;

        public GetIdentityClientCommandTest()
        {
            string createConnectionString(Guid instanceId, string path) => $@"Data Source=(LocalDb)\MSSQLLocalDB;database=IdentityShell.Test.{instanceId};trusted_connection=yes;AttachDBFilename={path}\IdentityShell-{instanceId}.mdf";

            this.connectionString = createConnectionString(
                instanceId: Guid.NewGuid(),
                path: Path.GetDirectoryName(typeof(GetIdentityClientCommandTest).GetTypeInfo().Assembly.Location));

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

        [Fact]
        public void IdentityShell_doesnt_have_premade_clients()
        {
            using var arrangeContext = this.serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            Assert.Empty(arrangeContext.Clients.ToList());
        }
    }
}