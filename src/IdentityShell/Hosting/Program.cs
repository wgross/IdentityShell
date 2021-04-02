// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

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
                // uncomment to write to Azure diagnostics stream
                //.WriteTo.File(
                //    @"D:\home\LogFiles\Application\identityserver.txt",
                //    fileSizeLimitBytes: 1_000_000,
                //    rollOnFileSizeLimit: true,
                //    shared: true,
                //    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            try
            {
                Log.Information("Starting host...");
                using var host = CreateHostBuilder(args).Build();
                host.Start();

                IdentityCommandBase.GlobalServiceProvider = host.Services;

                Log.Information("Starting powershell...");
                var iss = InitialSessionState
                    .CreateDefault()
                    .AddIdentityConfigurationCommands()
                    .AddCommonCommands()
                    .AddEndpointCommands();
                // .AddIdentityOperationCommands()
                //.AddAspIdentityCommands();
                //.AddWebHostCommands();

                iss.ExecutionPolicy = ExecutionPolicy.Unrestricted;

                ConsoleShell.Start(iss, "IdentityShell", "", args);
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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}