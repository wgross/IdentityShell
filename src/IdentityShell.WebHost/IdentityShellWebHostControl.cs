using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityShell.WebHost
{
    public class IdentityShellWebHostControl
    {
        public static CancellationTokenSource WebHostCancellationTokenSource { get; } = new CancellationTokenSource();

        public static Task WebHostTask { get; private set; }

        public static void StartWebHost(IHostBuilder builder)
        {
            Log.Information("Starting host...");

            WebHostTask = builder.Build().RunAsync(WebHostCancellationTokenSource.Token);
        }

        public static void StopWebHost()
        {
            Log.Information("Stopping host...");

            WebHostCancellationTokenSource.Cancel();
            WebHostTask.Wait();
            WebHostTask.Dispose();
        }
    }
}