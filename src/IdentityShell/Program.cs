// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityShell.Cmdlets.AspNetIdentity;
using IdentityShell.Cmdlets.Common;
using IdentityShell.Cmdlets.Configuration;
using IdentityShell.Cmdlets.IdentityEndpoints;
using IdentityShell.Cmdlets.Operation;
using IdentityShell.Cmdlets.WebHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.PowerShell;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
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

            var webHostControl = new IdentityShellWebHostControl();
            RestartIdentityServerCommand.WebHostControl = webHostControl;

            try
            {
                webHostControl.Start(args);

                var iss = InitialSessionState
                    .CreateDefault()
                    .AddIdentityConfigurationCommands()
                    .AddIdentityOperationCommands()
                    .AddCommonCommands()
                    .AddEndpointCommands()
                    .AddAspIdentityCommands()
                    .AddWebHostCommands();

                iss.ExecutionPolicy = ExecutionPolicy.Unrestricted;

                ConsoleShell.Start(iss, "IdentityShell", "", args);
            }
            finally
            {
                webHostControl.Stop();
                Log.CloseAndFlush();
            }
            return 0;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cfg => cfg.AddInMemoryCollection(IdentityCommandConfgurationOverride.Default))
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}