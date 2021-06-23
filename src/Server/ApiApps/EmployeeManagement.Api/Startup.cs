using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using EmployeeManagement.Api.Swagger;
using EmployeeManagement.Api.Utilities.Mixed;
using EmployeeManagement.Infrastructure.Services;
using EmployeeManagement.Persistence.Cache;
using EmployeeManagement.Persistence.RelationalDB.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerUI;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Api
{
    public class Startup
    {
        private readonly string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: myAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:44364")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
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

            string connectionName = WebHostEnvironment.IsDevelopment() ? "EmployeeDbConnection" : "EmployeeDbConnection";
            string connectionString = Configuration.GetConnectionString(connectionName);

            services.AddRelationalDbContext(connectionString);

            string sendGridApiKey = "yourSendGridKey";
            services.AddSendGrid(sendGridApiKey);

            services.AddCaching();

            services.AddServicesOfAllTypes("EmployeeManagement");
            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            });

            services.AddSwaggerGeneration();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Not appplicable here")]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });

            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error");
                ////app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

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

            app.UseResponseCompression();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(myAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
