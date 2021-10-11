using System.IO.Compression;
using Identity.Api.Configs;
using Identity.Api.Extensions;
using Identity.Api.Filters;
using Identity.Api.Swagger;
using Identity.Api.Utils;
using Identity.Application.Queries.UserQueries;
using Identity.Infrastructure.Services;
using Identity.Persistence.RelationalDB;
using Identity.Persistence.RelationalDB.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerUI;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace Identity.Api
{
    public class Startup
    {
        private readonly string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ////services.AddCors(options =>
            ////{
            ////    options.AddPolicy(
            ////        name: myAllowSpecificOrigins,
            ////        builder =>
            ////        {
            ////            builder
            ////            ////.WithOrigins("https://localhost:44364")
            ////            .AllowAnyOrigin()
            ////            .AllowAnyHeader()
            ////            .AllowAnyMethod();
            ////        });
            ////});

            services.AddCors(options =>
            {
                options.AddPolicy(myAllowSpecificOrigins, builder =>
                {
                    builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed(_ => true);
                });
            });

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(ExceptionHandlerFilter));
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    return new BadRequestObjectResult(context.ModelState);
                };
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.AddIdentityDbContext(Configuration.GetConnectionString("IdentityDbConnection"));

            JwtConfig jwtConfig = new JwtConfig("SampleIdentity.com", "SampleIdentitySecretKey", 86400);
            services.AddJwtAuthentication(jwtConfig);

            services.AddJwtTokenGenerator(jwtConfig);

            services.AddExternalLogins(Configuration);

            string sendGridApiKey = "yourSendGridKey";
            services.AddSendGrid(sendGridApiKey);

            services.AddMediatR(typeof(GetRefreshTokenQuery));

            services.AddServicesOfAllTypes();

            services.AddSwaggerGeneration();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.ApplyDatabaseMigrations();

            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseApiVersioning();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DocExpansion(DocExpansion.None);

                foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json", $"API {description.GroupName.ToUpperInvariant()}");
                }
            });

            app.UseResponseCompression();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(myAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
