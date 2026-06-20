using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using WiSave.Incomes.WebApi.Authorization;

namespace WiSave.Incomes.WebApi.OpenApi;

public sealed class DevelopmentApiKeySecurityTransformer : IOpenApiDocumentTransformer
{
    public const string SchemeName = "DevelopmentApiKey";

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        var securitySchemes = document.Components.SecuritySchemes
            ??= new Dictionary<string, IOpenApiSecurityScheme>();
        securitySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = DevelopmentApiKeyAuthentication.HeaderName,
            Description = "Development-only API key. Use appsettings.Development.json value: dev-incomes-key."
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SchemeName, document, null)] = []
        });

        return Task.CompletedTask;
    }
}
