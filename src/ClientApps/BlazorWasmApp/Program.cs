using System.Threading.Tasks;
using BlazorApps.Shared.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TanvirArjel.Blazor.DependencyInjection;

namespace BlazorWasmApp
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddComponents();

            builder.Services.AddSharedServices();

            await builder.Build().RunAsync();
        }
    }
}
