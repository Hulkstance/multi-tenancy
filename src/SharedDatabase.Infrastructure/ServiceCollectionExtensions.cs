using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SharedDatabase.Infrastructure.MultiTenancy;
using SharedDatabase.Infrastructure.Persistence;

namespace SharedDatabase.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddMultiTenancy()
            .AddPersistence();
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app
            .UseMultiTenancy();
    }
}
