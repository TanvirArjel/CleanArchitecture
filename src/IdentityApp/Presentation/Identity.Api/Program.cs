namespace Identity.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.WebHost.CaptureStartupErrors(true);

        builder.ConfigureServices();

        WebApplication webApp = builder.Build();

        webApp.ConfigureMiddlewares();

        webApp.Run();
    }
}
