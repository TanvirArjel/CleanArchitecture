using System.IO.Compression;
using EmployeeManagement.Api.Extensions;
using EmployeeManagement.Api.Filters;
using EmployeeManagement.Api.Swagger;
using EmployeeManagement.Api.Utilities;
using EmployeeManagement.Application.Commands.DepartmentCommands;
using EmployeeManagement.Infrastructure.Services;
using EmployeeManagement.Persistence.Cache;
using EmployeeManagement.Persistence.RelationalDB.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.ResponseCompression;
using Swashbuckle.AspNetCore.SwaggerUI;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Api;

public static class Startup
{
    private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    // This method gets called by the runtime. Use this method to add services to the container.
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        IServiceCollection services = builder.Services;

        services.AddCors(options =>
        {
            options.AddPolicy(
                name: MyAllowSpecificOrigins,
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

        string connectionName = builder.Environment.IsDevelopment() ? "EmployeeDbConnection" : "EmployeeDbConnection";
        string connectionString = builder.Configuration.GetConnectionString(connectionName);

        services.AddRelationalDbContext(connectionString);

        string sendGridApiKey = "yourSendGridKey";
        services.AddSendGrid(sendGridApiKey);

        services.AddCaching();

        services.AddMediatR(typeof(CreateDepratmentCommand));

        services.AddServicesOfAllTypes("EmployeeManagement");
        services.AddControllers(options =>
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

        services.AddSwaggerGeneration();

        services.AddJwtAuthentication();
    }

    public static void ConfigureMiddlewares(this WebApplication app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        app.ApplyDatabaseMigrations();

        app.Use((context, next) =>
        {
            context.Request.EnableBuffering();
            return next();
        });

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
        // specifying the Swagger JSON endpoint.
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.DocExpansion(DocExpansion.None);

            IApiVersionDescriptionProvider provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

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

        app.UseCors(MyAllowSpecificOrigins);

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
