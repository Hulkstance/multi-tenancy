using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedDatabase.Infrastructure.Auth;
using SharedDatabase.Infrastructure.Common.OpenApi;
using SharedDatabase.Infrastructure.MultiTenancy;
using SharedDatabase.Infrastructure.Persistence;

namespace SharedDatabase.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        return services
            .AddOpenApiDocumentation()
            .AddAuth(configuration)
            .AddAuthorization()
            .AddMultiTenancy()
            .AddPersistence()
            .AddExceptionHandler<CustomExceptionHandler>().AddProblemDetails();
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IHostEnvironment env)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(env);

        return app
            .UseOpenApiDocumentation(env)
            .UseAuthentication()
            .UseMultiTenancy()
            .UseAuthorization()
            .UseExceptionHandler();
    }

    public static IEndpointRouteBuilder MapInfrastructure(this IEndpointRouteBuilder endpoint, IHostEnvironment env)
    {
        ArgumentNullException.ThrowIfNull(endpoint);
        ArgumentNullException.ThrowIfNull(env);

        return endpoint
            .MapOpenApiDocumentation(env);
    }
}
