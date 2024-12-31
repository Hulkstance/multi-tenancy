using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedDatabase.Infrastructure.Auth.Jwt;

namespace SharedDatabase.Infrastructure.Auth;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // TODO: Select auth provider using configuration
        services.AddJwtAuth(configuration);

        return services;
    }
}
