using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Identity.Api.Swagger
{
    public static class SwaggerGenerationExtensions
    {
        public static void AddSwaggerGeneration(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddApiVersioning(config =>
            {
                // Specify the default API Version as 0.0
                config.DefaultApiVersion = new ApiVersion(0, 0);

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
                // JWT secutiry definition for swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Description = "Authorization header using the Bearer scheme",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer"
                });

                // Adding Bearer as default.
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

                // Grouping endopings by version and ApiExplorer group name.
                options.DocInclusionPredicate((documentName, apiDescription) =>
                {
                    ApiVersionModel actionApiVersionModel = apiDescription.ActionDescriptor
                    .GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

                    ApiExplorerSettingsAttribute apiExplorerSettingsAttribute = (ApiExplorerSettingsAttribute)apiDescription.ActionDescriptor
                    .EndpointMetadata.First(x => x.GetType().Equals(typeof(ApiExplorerSettingsAttribute)));

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

                // Grouping endpoings by ApiExplorer GroupName.
                options.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    ControllerActionDescriptor controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerActionDescriptor != null)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });

                // Adding all the available versions.
                IApiVersionDescriptionProvider apiVersionDescriptionProvider = services.BuildServiceProvider()
                .GetService<IApiVersionDescriptionProvider>();

                foreach (ApiVersionDescription description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    OpenApiInfo openApiInfo = new OpenApiInfo()
                    {
                        Title = $"Identity {description.GroupName} API",
                        Version = description.ApiVersion.ToString(),
                        Description = $"Identity {description.GroupName} API description."
                    };

                    if (description.IsDeprecated)
                    {
                        openApiInfo.Description += " This API version has been deprecated.";
                    }

                    options.SwaggerDoc(description.GroupName, openApiInfo);
                }

                // Adding swagger data annotation support.
                options.EnableAnnotations();
            });
        }
    }
}
