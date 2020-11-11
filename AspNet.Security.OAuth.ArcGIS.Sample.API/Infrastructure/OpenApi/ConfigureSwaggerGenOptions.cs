using AspNet.Security.OAuth.ArcGIS.Sample.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Security.OAuth.ArcGIS.Sample.API.Infrastructure.OpenApi
{
    public class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider provider;
        private readonly IConfiguration configuration;

        public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
        {
            this.provider = provider;
            this.configuration = configuration;
        }

        public void Configure(SwaggerGenOptions options)
        {
            options.OperationFilter<AuthorizeCheckOperationFilter>();
            options.DescribeAllParametersInCamelCase();
            options.CustomSchemaIds(x => x.FullName);
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateOpenApiInfoForApiVersion());
            }

            options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,

                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(this.configuration["Credentials:AuthorizationEndpoint"]),
                        TokenUrl = new Uri(this.configuration["Credentials:TokenEndpoint"]),
                        Scopes = new Dictionary<string, string>
                        {
                            { "api" , "SAMPLE Demo API - full access" }
                        },
                    }
                },
                Description = "SAMPLE Demo Server OpenId Security Scheme"
            });
        }

        private static OpenApiInfo CreateOpenApiInfoForApiVersion()
        {
            var info = new OpenApiInfo()
            {
                Version = "v1",
                Title = "SAMPLE API",
                Description = "API para probar la autenticación de ArcGIS",
                Contact = new OpenApiContact
                {
                    Name = "Rubén Astráin",
                    Email = "rastrain@tracasa.es"
                }
            };

            return info;
        }
    }
}
