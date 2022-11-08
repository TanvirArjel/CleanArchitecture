using System.Text;
using CleanHr.Api.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services, JwtConfig jwtConfig)
    {
        services.ThrowIfNull(nameof(services));
        jwtConfig.ThrowIfNull(nameof(jwtConfig));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key))
            };
        });
    }

    public static void AddJwtTokenGenerator(this IServiceCollection services, JwtConfig jwtConfig)
    {
        services.ThrowIfNull(nameof(services));
        jwtConfig.ThrowIfNull(nameof(jwtConfig));

        services.AddSingleton(jwtConfig);
    }

    public static void AddExternalLogins(this IServiceCollection services, IConfiguration configuration)
    {
        services.ThrowIfNull(nameof(services));
        configuration.ThrowIfNull(nameof(configuration));

        services.AddAuthentication()
        .AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = configuration.GetSection("ExternalLoginProviders:Google:ClientId").Value;
            googleOptions.ClientSecret = configuration.GetSection("ExternalLoginProviders:Google:ClientSecret").Value;
            googleOptions.SaveTokens = true;
        })
        ////.AddFacebook(facebookOptions =>
        ////{
        ////    facebookOptions.AppId = configuration.GetSection("ExternalLoginProviders:Facebook:AppId").Value;
        ////    facebookOptions.AppSecret = configuration.GetSection("ExternalLoginProviders:Facebook:AppSecret").Value;
        ////})
        .AddTwitter(twitterOptions =>
        {
            twitterOptions.ConsumerKey = configuration.GetSection("ExternalLoginProviders:Twitter:ConsumerKey").Value;
            twitterOptions.ConsumerSecret = configuration.GetSection("ExternalLoginProviders:Twitter:ConsumerSecret").Value;
            twitterOptions.SaveTokens = true;
        });
    }
}
