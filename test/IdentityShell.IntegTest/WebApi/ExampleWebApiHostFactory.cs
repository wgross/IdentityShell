using Example.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace IdentityShell.IntegTest.WebApi
{
    public class ExampleWebApiHostFactory : WebApplicationFactory<Example.WebApi.Program>
    {
        private readonly Microsoft.AspNetCore.TestHost.TestServer identityTestServer;

        public ExampleWebApiHostFactory(Microsoft.AspNetCore.TestHost.TestServer identityServer)
        {
            this.identityTestServer = identityServer;
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return Microsoft.Extensions.Hosting.Host
               .CreateDefaultBuilder()
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup(this.StartUpFactory);
               });
        }

        /// <summary>
        /// Inject the identity server test instance in to the startup configuration
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Startup StartUpFactory(WebHostBuilderContext arg)
            => new ExampleWebApiStartup(arg.Configuration, this.identityTestServer);
    }
}