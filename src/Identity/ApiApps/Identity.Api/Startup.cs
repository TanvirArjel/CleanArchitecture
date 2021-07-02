using System.IO.Compression;
using Identity.Api.Extensions;
using Identity.Api.Swagger;
using Identity.Api.Utils;
using Identity.Application;
using Identity.Infrastructure.Services;
using Identity.Persistence.RelationalDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerUI;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
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

            string sendGridApiKey = "yourSendGridKey";
            services.AddSendGrid(sendGridApiKey);

            services.AddServicesOfAllTypes();

            services.AddSwaggerGeneration();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.DocExpansion(DocExpansion.None);

                    // build a swagger endpoint for each discovered API version.
                    foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
                    {
                        options.RoutePrefix = "swagger";
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });
            }

            app.UseResponseCompression();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
