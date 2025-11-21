using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanHr.Api.Extensions;

internal static class SwaggerGenerationServicesExtensions
{
    public static void AddSwaggerGeneration(
        this IServiceCollection services,
        string apiName,
        string apiAssemblyName)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddApiVersioning(config =>
        {
            // Specify the default API Version as 1.0
            config.DefaultApiVersion = new ApiVersion(1, 0);

            // If the client hasn't specified the API version in the request, use the default API version number
            config.AssumeDefaultVersionWhenUnspecified = true;

            // Advertise the API versions supported for the particular endpoint
            config.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(options =>
        {
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'VVV";

            // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
            // can also be used to control the format of the API version in route templates
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddSwaggerGen(options =>
        {
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Description = "Authorization header using the Bearer scheme",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            options.TagActionsBy(api =>
            {
                if (api.GroupName != null)
                {
                    return new[] { api.GroupName };
                }

                if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    return new[] { controllerActionDescriptor.ControllerName };
                }

                throw new InvalidOperationException("Unable to determine tag for endpoint.");
            });

            // Grouping endpoints by version and ApiExplorer group name.
            options.DocInclusionPredicate((documentName, apiDescription) =>
            {
                ApiVersionModel actionApiVersionModel = apiDescription.ActionDescriptor
                .GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

                if (actionApiVersionModel == null)
                {
                    return true;
                }

                if (actionApiVersionModel.DeclaredApiVersions.Any())
                {
                    return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.MajorVersion}" == documentName);
                }

                return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.MajorVersion}" == documentName);
            });

            options.EnableAnnotations();
            string xmlCommentFilePath = Path.Combine(AppContext.BaseDirectory, $"{apiAssemblyName}.xml");
            options.IncludeXmlComments(xmlCommentFilePath);
        });

        // Adding all the available versions.
        services.ConfigureOptions<ConfigureSwaggerOptions>();
    }
}

internal sealed class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    private readonly string _apiName;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
        _apiName = "Clean HR";
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (ApiVersionDescription description in _provider.ApiVersionDescriptions)
        {
            OpenApiInfo openApiInfo = new()
            {
                Title = $"{_apiName} {description.GroupName.ToUpperInvariant()} API Endpoints",
                Version = description.ApiVersion.ToString(),
                Description = $"{_apiName} {description.GroupName} API endpoints descriptions."
            };

            if (description.IsDeprecated)
            {
                openApiInfo.Description += " This API version has been deprecated.";
            }

            options.SwaggerDoc(description.GroupName, openApiInfo);
        }
    }
}
