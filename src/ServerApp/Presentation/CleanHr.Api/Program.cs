using CleanHr.Api.Extensions;
using Serilog;

namespace CleanHr.Api;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            // Configure Serilog
            SerilogConfiguration.ConfigureSerilog();

            Log.Information("Initializing CleanHR API application...");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Use Serilog for logging
            builder.Host.UseSerilog();

            builder.WebHost.CaptureStartupErrors(true);

            Log.Information("Configuring OpenTelemetry tracing...");
            // Configure OpenTelemetry Tracing
            builder.Services.AddOpenTelemetryTracing(builder.Configuration);

            Log.Information("Configuring application services...");
            // Configure  application services
            builder.ConfigureServices();

            Log.Information("Building web application...");
            // Build the web application.
            WebApplication webApp = builder.Build();

            Log.Information("Configuring middleware pipeline...");
            // Add middlewares to the web application.
            await webApp.ConfigureMiddlewaresAsync();

            Log.Information("Starting web host...");

            // Run the web application.
            await webApp.RunAsync();

            Log.Information("CleanHR API application stopped gracefully");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "CleanHR API application failed to start");
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == Environments.Development)
            {
                throw;
            }
        }
        finally
        {
            Log.Information("Shutting down CleanHR API application...");
            await Log.CloseAndFlushAsync();
        }
    }
}
