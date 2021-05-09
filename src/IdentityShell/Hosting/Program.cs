using IdentityShell.Commands;
using IdentityShell.Commands.Common;
using IdentityShell.Commands.Configuration;
using IdentityShell.Commands.Endpoints;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.PowerShell;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace IdentityShell
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            RootCommand rootCommand = new();
            rootCommand.Add(new Option<bool>(new[] { "--non-interactive", "-ni" }, "Start without an interactive shell "));
            rootCommand.Add(new Option<string[]>(new[] { "--run-script", "-s" }, "Startup script(s)"));
            rootCommand.Description = "Run openid connect server";
            rootCommand.Handler = CommandHandler.Create<bool, string[]>(async (nonInteractive, runScript)
                => await StartIdentityShell(nonInteractive, runScript, args));

            try
            {
                return rootCommand.Invoke(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static async Task StartIdentityShell(bool nonInteractive, string[] runScript, string[] args)
        {
            using var webHost = await StartWebHost(args);

            // share service provider with identity commands
            IdentityCommandBase.GlobalServiceProvider = webHost.Services;

            if (nonInteractive)
            {
                StartNonInteractiveShell(runScript.ToArray());
            }
            else
            {
                StartInteractiveShell(runScript.ToArray());

                await webHost.StopAsync();
            }

            await webHost.WaitForShutdownAsync();
        }

        private async static Task<IHost> StartWebHost(string[] args)
        {
            Log.Information("Starting identity server...");

            var host = CreateHostBuilder(args).Build();

            await host.StartAsync();

            return host;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

        private static void StartNonInteractiveShell(string[] startupScripts)
        {
            Log.Information($"Running script '{string.Join(",", startupScripts)}'...");

            RunStartupScripts(startupScripts, CreateInitialSessionState());
        }

        private static void StartInteractiveShell(string[] startupScripts)
        {
            InitialSessionState iss = CreateInitialSessionState();

            RunStartupScripts(startupScripts, iss);

            Log.Information("Starting identity server powershell...");

            ConsoleShell.Start(iss, "IdentityShell", "", Array.Empty<string>());
        }

        private static void RunStartupScripts(string[] startupScripts, InitialSessionState iss)
        {
            if (startupScripts.Any())
            {
                Log.Information("Initializing identity server powershell...");

                var powershell = PowerShell.Create(iss);

                foreach (var s in startupScripts)
                {
                    Log.Information("Adding script {startupScript}", s);

                    powershell.AddCommand(s);
                }

                Log.Information("Invoking scripts");
                powershell.Invoke();
            }
        }

        private static InitialSessionState CreateInitialSessionState()
        {
            var sessionState = InitialSessionState
                .CreateDefault()
                .AddIdentityConfigurationCommands()
                .AddCommonCommands()
                .AddEndpointCommands();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                sessionState.ExecutionPolicy = ExecutionPolicy.Unrestricted;
            }

            return sessionState;
        }
    }
}