using System;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using MauiBlazor.Shared.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace MauiBlazor.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSharedServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);

            services.AddOptions();
            services.AddAuthorizationCore();
            services.AddScoped<HostAuthStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<HostAuthStateProvider>());

            services.AddScoped<JwtSecurityTokenHandler>();

            services.AddServicesOfAllTypes(typeof(JwtTokenParser).Assembly);

            services.AddHttpClient("IdentityApi", c =>
            {
                c.BaseAddress = new Uri("https://localhost:44363/api/");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            }).AddHttpMessageHandler<AuthorizationDelegatingHandler>();

            services.AddHttpClient("EmployeeManagementApi", c =>
            {
                c.BaseAddress = new Uri("https://localhost:44390/api/");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            }).AddHttpMessageHandler<AuthorizationDelegatingHandler>();
        }
    }
}
