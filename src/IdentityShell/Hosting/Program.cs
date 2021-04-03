// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using IdentityShell.Commands;
using IdentityShell.Commands.Common;
using IdentityShell.Commands.Configuration;
using IdentityShell.Commands.Endpoints;
using IdentityShell.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.PowerShell;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

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

            try
            {
                Log.Information("Starting host...");
                using var host = CreateHostBuilder(args).Build();
                host.Start();

                var commandlineConfig = host.Services.GetRequiredService<IOptions<CommandLineOptions>>();

                IdentityCommandBase.GlobalServiceProvider = host.Services;

                if (string.IsNullOrEmpty(commandlineConfig.Value.StartupConfiguration))
                {
                    StartInteractiveShell(Array.Empty<string>());
                }
                else if (commandlineConfig.Value.HideShell == false)
                {
                    StartInteractiveShell(new[] { "-NoExit", "-c", commandlineConfig.Value.StartupConfiguration });
                }
                else
                {
                    StartNonInteractiveShell(commandlineConfig.Value.StartupConfiguration);
                    
                    host.WaitForShutdown();
                }
                return 0;
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

        private static void StartNonInteractiveShell(string startupScript)
        {
            Log.Information($"Running script '{startupScript}'...");
            var sessionState = CreateInitialSessionState();
            sessionState.ExecutionPolicy = ExecutionPolicy.Unrestricted;

            PowerShell
                .Create(sessionState)
                .AddCommand(startupScript)
                .Invoke();
        }

        private static void StartInteractiveShell(string[] args)
        {
            Log.Information("Starting powershell...");
            InitialSessionState iss = CreateInitialSessionState();

            iss.ExecutionPolicy = ExecutionPolicy.Unrestricted;

            ConsoleShell.Start(iss, "IdentityShell", "", args);
        }

        private static InitialSessionState CreateInitialSessionState() => InitialSessionState
            .CreateDefault()
            .AddIdentityConfigurationCommands()
            .AddCommonCommands()
            .AddEndpointCommands();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}