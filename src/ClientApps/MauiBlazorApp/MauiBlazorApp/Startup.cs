using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Blazored.LocalStorage;
using ClientApps.Shared.Common;
using ClientApps.Shared.Services;
using MauiBlazorApp.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace MauiBlazorApp
{
    public class Startup : IStartup
    {
        public void Configure(IAppHostBuilder appBuilder)
        {
            appBuilder
                .RegisterBlazorMauiWebView(typeof(Startup).Assembly)
                .UseMicrosoftExtensionsServiceProviderFactory()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .ConfigureServices(services =>
                {
                    services.AddBlazorWebView();
                    services.AddSingleton<WeatherForecastService>();

                    services.AddOptions();
                    services.AddAuthorizationCore();
                    services.AddScoped<HostAuthStateProvider>();
                    services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<HostAuthStateProvider>());

                    services.AddScoped<JwtSecurityTokenHandler>();

                    services.AddServicesOfAllTypes(typeof(UserService).Assembly);
                    services.AddServicesOfAllTypes(Assembly.GetExecutingAssembly());
                    services.AddHttpClient("EmployeeManagementApi", c =>
                    {
                        c.BaseAddress = new Uri("https://localhost:44390/api/");
                        c.DefaultRequestHeaders.Add("Accept", "application/json");
                        c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
                    }).AddHttpMessageHandler<AuthorizationDelegatingHandler>();

                    services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);

                    services.AddHttpClient("IdentityApi", c =>
                    {
                        c.BaseAddress = new Uri("https://localhost:44363/api/");
                        c.DefaultRequestHeaders.Add("Accept", "application/json");
                        c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
                    }).AddHttpMessageHandler<AuthorizationDelegatingHandler>();
                });
        }
    }
}