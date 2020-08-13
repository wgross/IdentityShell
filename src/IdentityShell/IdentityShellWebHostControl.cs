using IdentityShell.Cmdlets.WebHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityShell
{
    public class IdentityShellWebHostControl : IWebHostControl
    {
        private CancellationTokenSource WebHostCancellationTokenSource { get; set; } = default;

        private Task WebHostTask { get; set; }

        public void Start(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cfg => cfg.AddInMemoryCollection(IdentityCommandConfgurationOverride.Default))
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

            this.Start(builder);
        }

        private void Start(IHostBuilder builder)
        {
            Log.Information("Starting host...");

            this.WebHostCancellationTokenSource = new CancellationTokenSource();
            this.WebHostTask = builder.Build().RunAsync(this.WebHostCancellationTokenSource.Token);
        }

        public void Stop()
        {
            Log.Information("Stopping host...");

            WebHostCancellationTokenSource.Cancel();
            WebHostTask.Wait();
            WebHostTask.Dispose();
            WebHostTask = null;
        }
    }
}