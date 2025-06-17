using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace FeatureBasedFolderStructure.API.Extensions;

internal sealed class AddHeaderParameterOpenApiOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Correlation-ID",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string"
            },
            Description = "Correlation ID for tracking requests across services."
        });
        
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Request-Idempotency-Key",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string"
            },
            Description = "Idempotency key for ensuring the request is processed only once."
        });
        
        return Task.CompletedTask;
    }
}
