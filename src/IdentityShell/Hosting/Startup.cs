// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Duende.IdentityServer;
using IdentityServerHost.Quickstart.UI;
using IdentityShell.Configuration;
using IdentityShell.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace IdentityShell
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // configure options for Commandline parameters
            services.Configure<CommandLineOptions>(this.Configuration);

            // read UI from embedded resources
            var embeddedResources = new ManifestEmbeddedFileProvider(typeof(Program).Assembly, "wwwroot");
            this.Environment.WebRootFileProvider = embeddedResources;
            services.AddSingleton<IFileProvider>(embeddedResources);

            // Add repos for inmemory config access
            IdentityServerInMemoryConfig inMemoryConfig = new();
            services.AddSingleton(inMemoryConfig);
            services.AddScoped<IIdentityResourceRepository, IdentityResourceRepository>();
            services.AddScoped<IApiResourceRepository, ApiResourceRepository>();
            services.AddScoped<IApiScopeRepository, ApiScopeRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ITestUserRepository, TestUserRepository>();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v5/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            }).AddTestUsers(TestUsers.Users);

            // in-memory, code config

            builder.AddInMemoryIdentityResources(inMemoryConfig.IdentityResources);
            builder.AddInMemoryApiScopes(inMemoryConfig.ApiScopes);
            builder.AddInMemoryClients(inMemoryConfig.Clients);
            builder.AddInMemoryApiResources(inMemoryConfig.ApiResources);
            builder.AddTestUsers(inMemoryConfig.TestUsers);

            services
                .AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:7777/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}