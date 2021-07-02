using System.Text;
using Identity.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TanvirArjel.ArgumentChecker;

namespace Identity.Api.Extensions
{
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
    }
}
