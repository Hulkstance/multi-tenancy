using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SharedDatabase.Infrastructure.Common.OpenApi;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        return services;
    }

    internal static IApplicationBuilder UseOpenApiDocumentation(this IApplicationBuilder app, IHostEnvironment env)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(env);

        if (env.IsDevelopment())
        {
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1/openapi.json", "Example API");
                options.RoutePrefix = "";
            });
        }

        return app;
    }

    internal static IEndpointRouteBuilder MapOpenApiDocumentation(this IEndpointRouteBuilder endpoint, IHostEnvironment env)
    {
        ArgumentNullException.ThrowIfNull(endpoint);
        ArgumentNullException.ThrowIfNull(env);

        if (env.IsDevelopment())
        {
            endpoint.MapOpenApi("/openapi/v1/openapi.json");
        }

        return endpoint;
    }
}
