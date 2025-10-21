using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Asp.Versioning.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChildcareManagementERP.Api.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo()
                {
                    Title = $"API Gestion Associatif ({description.GroupName.ToUpper()})",
                    Version = description.ApiVersion.ToString(),
                    Description = "Documentation versionnée via Swagger UI",
                });
            }
        }
    }
}