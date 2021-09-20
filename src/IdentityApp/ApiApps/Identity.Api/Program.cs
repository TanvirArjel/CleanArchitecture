using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Identity.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

            hostBuilder.Build().Run();
        }
    }
}
