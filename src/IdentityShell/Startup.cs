// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.EntityFramework.DbContexts;
using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;
using IdentityShell.Cmdlets.Configuration;
using IdentityShell.IdentityStore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace IdentityShell
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public IConfiguration Configuration { get; }

        public static IServiceProvider AppServices { get; private set; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.Environment = environment;
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();

            services
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    // dotnet ef migrations add CreateIdentitySchema -c ApplicationDbContext -o Data/Migrations -s ..\IdentityShell\IdentityShell.csproj
                    options.UseSqlite(Configuration.GetConnectionString("UserStore"));
                })
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var builder = services
                .AddIdentityServer()
                .AddConfigurationStore(options =>
                {
                    // dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb -s ..\IdentityShell\IdentityShell.csproj
                    string migrationsAssembly = typeof(ApiResourceUpdateMappers).GetTypeInfo().Assembly.GetName().Name;

                    options.ConfigureDbContext = builder => builder.UseSqlite(this.Configuration.GetConnectionString("ConfigurationStore"), sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    // dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb -s ..\IdentityShell\IdentityShell.csproj
                    string migrationsAssembly = typeof(ApiResourceUpdateMappers).GetTypeInfo().Assembly.GetName().Name;

                    options.ConfigureDbContext = b => b.UseSqlite(this.Configuration.GetConnectionString("OperationalStore"), sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddAspNetIdentity<ApplicationUser>();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            this.InitializeDatabase(app);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseIdentityServer();

            AppServices = app.ApplicationServices;

            // inject the scope factory in the cmdlet base class
            IdentityConfigurationCommandBase.GlobalServiceProvider = app.ApplicationServices;
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            using var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            using var operationalContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
            using var userContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            configurationContext.Database.Migrate();
            operationalContext.Database.Migrate();
            userContext.Database.Migrate();
        }
    }
}