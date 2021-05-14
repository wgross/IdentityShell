using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityShell.Hosting
{
    public class IdentityServerHostingService : BackgroundService
    {
        private readonly IConfiguration powershellHostConfiguration;
        private readonly IServiceProvider powershellServiceProvider;
        private readonly ILogger<IdentityServerHostingService> logger;

        public IdentityServerHostingService(
            IConfiguration powershellHostConfiguration,
            IServiceProvider powershellServiceProvider,
            ILogger<IdentityServerHostingService> logger)
        {
            this.powershellHostConfiguration = powershellHostConfiguration;
            this.powershellServiceProvider = powershellServiceProvider;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("Starting identity server...");

            using var identityServerHost = Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddConfiguration(this.powershellHostConfiguration);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup(ctx => new Startup(ctx.HostingEnvironment, this.powershellHostConfiguration, this.powershellServiceProvider));
                })
                .Build();
            await identityServerHost.RunAsync(stoppingToken);
        }
    }
}