using System.IO.Compression;
using EmployeeManagement.Api.Configs;
using EmployeeManagement.Api.Extensions;
using EmployeeManagement.Api.Filters;
using EmployeeManagement.Api.Utilities;
using EmployeeManagement.Application.Commands.DepartmentCommands;
using EmployeeManagement.Infrastructure.Services;
using EmployeeManagement.Persistence.Cache;
using EmployeeManagement.Persistence.RelationalDB.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Api;

public static class Startup
{
    private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    private static bool InDocker => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

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
                    builder.WithOrigins("https://localhost:44364", "http://localhost:7300")
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

        string connectionString = string.Empty;

        if (InDocker)
        {
            connectionString = builder.Configuration.GetConnectionString("DockerDbConnection");
        }
        else
        {
            string connectionName = builder.Environment.IsDevelopment() ? "EmployeeDbConnection" : "EmployeeDbConnection";
            connectionString = builder.Configuration.GetConnectionString(connectionName);
        }

        services.AddRelationalDbContext(connectionString);

        string sendGridApiKey = "yourSendGridKey";
        services.AddSendGrid(sendGridApiKey);

        services.AddCaching();

        services.AddMediatR(typeof(CreateDepartmentCommand));

        services.AddServicesOfAllTypes("EmployeeManagement");
        services.AddControllersWithViews(options =>
        {
            options.Filters.Add(typeof(BadRequestResultFilter));
            options.Filters.Add(typeof(ExceptionHandlerFilter));
            options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
        });

        services.AddSwaggerGeneration("Employee Management", "EmployeeManagement.Api");

        JwtConfig jwtConfig = new JwtConfig("SampleIdentity.com", "SampleIdentitySecretKey", 86400);
        services.AddJwtAuthentication(jwtConfig);

        services.AddJwtTokenGenerator(jwtConfig);

        services.AddExternalLogins(builder.Configuration);
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

        app.UseSerilogRequestLogging();

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
