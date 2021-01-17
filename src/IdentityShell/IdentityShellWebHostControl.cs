using IdentityShell.Cmdlets.WebHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityShell.Hosting
{
    public class IdentityShellWebHostControl : IWebHostControl
    {
        private CancellationTokenSource WebHostCancellationTokenSource { get; set; } = default;

        private Task WebHostTask { get; set; }

        #region Control interactive lifecycle

        public void Start(string[] args) => this.Start(this.Build(args));

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

        #endregion Control interactive lifecycle
         
        #region Non-interactive lifecycle

        public void Run(string[] args) => Build(args).Build().Run();

        #endregion Non-interactive lifecycle

        private IHostBuilder Build(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cfg => cfg.AddInMemoryCollection(IdentityCommandConfgurationOverride.Default))
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}