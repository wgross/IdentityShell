// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityShell.Commands;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.PowerShell;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityShell
{
    public class Program
    {
        private readonly static CancellationTokenSource webHostCancellationTokenSource = new CancellationTokenSource();
        private static Task webHostTask;

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
                webHostTask = StartWebHost(args);

                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.Commands.Add(new SessionStateCmdletEntry("Get-IdentityClient", typeof(GetIdentityClientCommand), string.Empty));
                iss.Commands.Add(new SessionStateCmdletEntry("Set-IdentityClient", typeof(SetIdentityClientCommand), string.Empty));
                iss.ExecutionPolicy = ExecutionPolicy.Unrestricted;
                iss.Variables.Add(new SessionStateVariableEntry("webHostTask", webHostTask, "Task executing the webhost"));
                ConsoleShell.Start(iss, "IdentityShell", "", args);
            }
            finally
            {
                webHostCancellationTokenSource.Cancel();
                webHostTask.Wait();
                webHostTask.Dispose();
                Log.CloseAndFlush();
            }
            return 0;
        }

        private static Task StartWebHost(string[] args)
        {
            Log.Information("Starting host...");
            return CreateHostBuilder(args).Build().RunAsync(webHostCancellationTokenSource.Token);
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