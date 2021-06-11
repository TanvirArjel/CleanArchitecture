using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TanvirArjel.Blazor.DependencyInjection;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace BlazorWasmApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddServicesOfAllTypes(Assembly.GetExecutingAssembly());
            builder.Services.AddHttpClient("EmployeeManagementApi", c =>
            {
                c.BaseAddress = new Uri("https://localhost:44390/api/");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            });

            builder.Services.AddComponents();

            await builder.Build().RunAsync();
        }
    }
}
