using CleanHr.Api.Extensions;
using Serilog;

namespace CleanHr.Api;

internal static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            // Configure Serilog
            SerilogConfiguration.ConfigureSerilog();

            Log.Information("Starting CleanHR API application");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Use Serilog for logging
            builder.Host.UseSerilog();

            builder.WebHost.CaptureStartupErrors(true);

            // Configure OpenTelemetry Tracing
            builder.Services.AddOpenTelemetryTracing(builder.Configuration);

            // Configure  application services
            builder.ConfigureServices();

            // Build the web application.
            WebApplication webApp = builder.Build();

            // Add middlewares to the web application.
            webApp.ConfigureMiddlewares();

            // Run the web application.
            webApp.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
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
