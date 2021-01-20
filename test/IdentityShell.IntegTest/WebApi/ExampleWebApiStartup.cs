using Example.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IdentityShell.IntegTest.WebApi
{
    public class ExampleWebApiStartup : Startup
    {
        private readonly Microsoft.AspNetCore.TestHost.TestServer identityTestServer;

        public ExampleWebApiStartup(IConfiguration configuration, Microsoft.AspNetCore.TestHost.TestServer identityTestServer)
            : base(configuration)
        {
            this.identityTestServer = identityTestServer;
        }

        protected override void ConfigureAuthenticationServices(IServiceCollection services)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOptions: c =>
                {
                    c.Authority = this.identityTestServer.BaseAddress.AbsoluteUri;
                    c.BackchannelHttpHandler = this.identityTestServer.CreateHandler();
                    c.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                    c.RequireHttpsMetadata = false;
                });
        }
    }
}