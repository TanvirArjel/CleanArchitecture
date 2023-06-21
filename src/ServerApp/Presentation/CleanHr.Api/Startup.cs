using System.IO.Compression;
using CleanHr.Api.Configs;
using CleanHr.Api.Extensions;
using CleanHr.Api.Filters;
using CleanHr.Api.Utilities;
using CleanHr.Application.Commands.DepartmentCommands;
using CleanHr.Infrastructure.Services;
using CleanHr.Persistence.Cache;
using CleanHr.Persistence.RelationalDB.Extensions;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Api;

public static class Startup
{
	private const string _myAllowSpecificOrigins = "_myAllowSpecificOrigins";

	private static bool InDocker => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

	// This method gets called by the runtime. Use this method to add services to the container.
	public static void ConfigureServices(this WebApplicationBuilder builder)
	{
		if (builder == null)
		{
			throw new ArgumentNullException(nameof(builder));
		}

		IServiceCollection services = builder.Services;
		services.AddHealthChecks().AddCheck<DbConnectionHealthCheck>("Database");

		services.AddCors(options =>
		{
			options.AddPolicy(
				name: _myAllowSpecificOrigins,
				builder =>
				{
					builder.WithOrigins("http://localhost:5200", "https://localhost:5201")
					.AllowAnyHeader()
					.AllowAnyMethod();
				});
		});

		////services.AddCors();

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

		string connectionString = builder.GetDbConnectionString();
		services.AddRelationalDbContext(connectionString);

		string sendGridApiKey = "yourSendGridKey";
		services.AddSendGrid(sendGridApiKey);

		services.AddCaching();

		services.AddMediatR(typeof(CreateDepartmentCommand));

		services.AddServicesOfAllTypes("CleanHr");
		services.AddControllersWithViews(options =>
		{
			options.Filters.Add(typeof(BadRequestResultFilter));
			options.Filters.Add(typeof(ExceptionHandlerFilter));
			options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
		});

		services.AddSwaggerGeneration("Clean HR", "CleanHr.Api");

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

		// If you are using http url then don't enable https redirection.
		////app.UseHttpsRedirection();

		app.MapHealthChecks("/healthz", new HealthCheckOptions()
		{
			ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
		});

		app.UseRouting();

		app.UseCors(_myAllowSpecificOrigins);
		////app.UseCors(options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());

		app.UseAuthentication();
		app.UseAuthorization();

		// Add controller routes
		app.MapControllers();
	}
}
