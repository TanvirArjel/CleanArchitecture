using Serilog;

namespace CleanHr.Api;

public static class Program
{
    ////private static readonly IConfiguration _configuration = new ConfigurationBuilder()
    ////        .SetBasePath(Directory.GetCurrentDirectory())
    ////        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    ////        .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true).Build();

    public static void Main(string[] args)
    {
        try
        {
            ////Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(_configuration).CreateLogger();

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.WebHost.CaptureStartupErrors(true);

            // Add Serilog
            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.MinimumLevel.Information() // Set the minimum log level
                    .WriteTo.Map(evt => evt.Level, (level, wt) => wt.File($"Logs//{level}-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7, formatProvider: null))
                    .Enrich.FromLogContext()
                    .WriteTo.Console(formatProvider: null);
            });

            // Configure  application services
            builder.ConfigureServices();

            // Build the web application.
            WebApplication webApp = builder.Build();

            // Add middlewares to the web application.
            webApp.ConfigureMiddlewares();

            // Run the web application.
            Log.Information("Starting web host");

            webApp.Run();

            Log.Information("Application started........");
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
