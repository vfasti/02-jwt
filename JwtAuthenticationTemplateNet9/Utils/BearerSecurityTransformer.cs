using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace JwtAuthenticationTemplateNet9.Helper;

/// <summary>
/// Fügt ein HTTP Bearer-Security-Schema + globales Requirement hinzu,
/// damit Swagger UI den "Authorize"-Dialog für Bearer anzeigt.
/// </summary>
sealed class BearerSecurityTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument doc, OpenApiDocumentTransformerContext ctx, CancellationToken ct)
    {
        doc.Components ??= new OpenApiComponents(); // wenn null, dann initialisieren
        doc.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

        doc.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer {token}'"
        };

        doc.SecurityRequirements ??= new List<OpenApiSecurityRequirement>(); 
        doc.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme { 
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } 
            }] = Array.Empty<string>()
        });

        return Task.CompletedTask;
    }
}
