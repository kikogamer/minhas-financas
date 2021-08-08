using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using minhas_financas.api.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace minhas_financas.api.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<SwaggerDefaultValues>();

                var security = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>()
                    }
                };

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT desta maneira: Bearer {seu token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(security);
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app, 
                                                                  IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                                            description.GroupName.ToUpperInvariant());
                }
            });

            return app;
        }
    }

    [ExcludeFromCodeCoverage]
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider provider;
        private readonly AppSettings _appSettings;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IOptions<AppSettings> appSettings)
        {
            this.provider = provider;
            _appSettings = appSettings.Value;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerGeneratorOptions.SwaggerDocs.Add(
                    description.GroupName, 
                    CreateInfoForApiVersion(description, _appSettings.LicenseURI)
                );
            }
        }

        static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description, string licenseURI)
        {
            var info = new OpenApiInfo()
            {
                Title = "API - Minhas Finanças Web",
                Version = description.ApiVersion.ToString(),
                Description = "API do Web App Minhas Finanças Web.",
                Contact = new OpenApiContact() { Name = "Ronaldo Carneiro", Email = "kikogamer@gmail.com" },
                TermsOfService = new System.Uri(licenseURI),
                License = new OpenApiLicense() { Name = "MIT", Url = new System.Uri(licenseURI) }
            };

            if (description.IsDeprecated)
            {
                info.Description = "Esta versão está obsoleta!";
            }

            return info;
        }
    }

    [ExcludeFromCodeCoverage]
    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;
            operation.Deprecated = apiDescription.IsDeprecated();

            if (operation.Parameters == null) return;

            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                if (parameter.Description == null)
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }

                if (parameter.Schema.Default == null)
                {
                    parameter.Schema.Default = OpenApiAnyFactory.CreateFor(parameter.Schema, description.DefaultValue);
                }

                parameter.Required |= description.IsRequired;
            }
        }
    }
}
