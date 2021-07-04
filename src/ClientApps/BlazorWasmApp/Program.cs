using System;
using System.Reflection;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BlazorWasmApp.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TanvirArjel.Blazor.DependencyInjection;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace BlazorWasmApp
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<HostAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<HostAuthStateProvider>());

            builder.Services.AddServicesOfAllTypes(Assembly.GetExecutingAssembly());
            builder.Services.AddHttpClient("EmployeeManagementApi", c =>
            {
                c.BaseAddress = new Uri("https://localhost:44390/api/");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            }).AddHttpMessageHandler<AuthorizationDelegatingHandler>();

            builder.Services.AddComponents();

            builder.Services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);

            builder.Services.AddHttpClient("IdentityApi", c =>
            {
                c.BaseAddress = new Uri("https://localhost:44363/api/");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            }).AddHttpMessageHandler<AuthorizationDelegatingHandler>();

            await builder.Build().RunAsync();
        }
    }
}
