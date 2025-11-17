using System;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using CleanHr.Blazor.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Blazor.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void AddDependencyServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);

        services.AddOptions();
        services.AddAuthorizationCore();
        services.AddScoped<HostAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<HostAuthStateProvider>());

        services.AddScoped<JwtSecurityTokenHandler>();

        services.AddServicesOfAllTypes(typeof(JwtTokenParser).Assembly);

        services.AddHttpClient("EmployeeManagementApi", c =>
        {
            c.BaseAddress = new Uri("http://localhost:5100/api/");
            c.DefaultRequestHeaders.Add("Accept", "application/json");
            c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
        }).AddHttpMessageHandler<AuthorizationDelegatingHandler>();
    }
}
