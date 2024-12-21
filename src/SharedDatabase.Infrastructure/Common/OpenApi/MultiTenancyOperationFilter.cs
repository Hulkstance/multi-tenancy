using Microsoft.OpenApi.Models;
using SharedDatabase.Infrastructure.MultiTenancy;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SharedDatabase.Infrastructure.Common.OpenApi;

public class MultiTenancyOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = MultiTenancyConstants.TenantIdHeader,
            In = ParameterLocation.Header,
            Description = "Tenant ID for multi-tenancy",
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string"
            }
        });
    }
}
