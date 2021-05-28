using IdentityShell.Commands;
using IdentityShell.Commands.Common;
using IdentityShell.Commands.Configuration;
using IdentityShell.Commands.Endpoints;
using IdentityShell.Configuration;
using IdentityShell.Hosting;
using IdentityShell.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.PowerShell;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace IdentityShell
{
    public class Program
    {
        public static IHost PowershellHost { get; private set; }

        public static int Main(string[] args)
        {
            Log.Logger = ConfigureSerilog();

            // share the servies with the cmdlets

            RootCommand rootCommand = CreateRootCommand();

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
                PowershellHost.Dispose();

                Log.CloseAndFlush();
            }
        }

        private static RootCommand CreateRootCommand()
        {
            RootCommand rootCommand = new();
            rootCommand.Add(new Option<bool>(new[] { "--non-interactive", "-ni" }, "Start without an interactive shell "));
            rootCommand.Add(new Option<string[]>(new[] { "--run-script", "-s" }, "Startup script(s)"));
            rootCommand.Handler = CommandHandler.Create<bool, string[], ParseResult>(StartIdentityShell);
            return rootCommand;
        }

        private static ILogger ConfigureSerilog()
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .WriteTo.Observers(log => log.Subscribe(LogEventsSink))
                .CreateLogger();

            return logger;
        }

        public static LogEventsSink LogEventsSink { get; } = new LogEventsSink();

        #region Start PowerShell

        private static async Task StartIdentityShell(bool nonInteractive, string[] runScript, ParseResult parseResult)
        {
            PowershellHost = CreatePowerShellHost(parseResult.UnparsedTokens);
            IdentityCommandBase.GlobalServiceProvider = PowershellHost.Services;

            await PowershellHost.StartAsync().ConfigureAwait(false);

            if (nonInteractive)
            {
                StartNonInteractiveShell(runScript.ToArray());

                // Identity Server host is shutting down already on Ctrl+C, do the same here
                Console.CancelKeyPress += (sender, e) => PowershellHost.StopAsync().ConfigureAwait(false);

                await PowershellHost.WaitForShutdownAsync().ConfigureAwait(false);
            }
            else
            {
                StartInteractiveShell(runScript.ToArray());

                await PowershellHost.StopAsync().ConfigureAwait(false);
            }
        }

        private static IHost CreatePowerShellHost(IReadOnlyList<string> args)
        {
            return Host
                .CreateDefaultBuilder(args.ToArray())
                .ConfigureServices(services =>
                {
                    // add log events sink for later retrieval
                    // services.AddSingleton(LogEventsSink);

                    // the backing config for the repositories is shared
                    services.AddSingleton(new IdentityServerInMemoryConfig());

                    // scope is the powershell pipeline
                    services.AddScoped<IIdentityResourceRepository, IdentityResourceRepository>();
                    services.AddScoped<IApiResourceRepository, ApiResourceRepository>();
                    services.AddScoped<IApiScopeRepository, ApiScopeRepository>();
                    services.AddScoped<IClientRepository, ClientRepository>();
                    services.AddScoped<ITestUserRepository, TestUserRepository>();

                    // runs the web service
                    services.AddHostedService<IdentityServerHostingService>();
                })
                .UseSerilog()
                .UseConsoleLifetime()
                .Build();
        }

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

        #endregion Start PowerShell
    }
}