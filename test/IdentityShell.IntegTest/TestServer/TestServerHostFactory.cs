using IdentityShell.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace IdentityShell.IntegTest.TestServer
{
    public class TestServerHostFactory : WebApplicationFactory<IdentityShell.Hosting.Program>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            return Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup(this.StartUpFactory);
                });
        }

        private Startup StartUpFactory(WebHostBuilderContext arg) => new TestServerStartup(arg.HostingEnvironment, arg.Configuration);
    }
}