using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EmployeeManagement.Api
{
    public static class Program
    {
        private static readonly IConfiguration _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true).Build();

        public static void Main(string[] args)
        {
            try
            {
                ////Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(_configuration).CreateLogger();

                Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information() // Set the minimun log level
                .WriteTo.Map(evt => evt.Level, (level, wt) => wt.File($"Logs\\{level}-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7))
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

                Log.Information("Starting web host");

                IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.CaptureStartupErrors(true);
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                });

                IHost host = hostBuilder.Build();
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (environment == Environments.Development)
                {
                    throw;
                }
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
