using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharedDatabase.Infrastructure.Persistence;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("Database");
            options.UseSqlServer(connectionString, x =>
            {
                x.MigrationsHistoryTable("EFMigrations", SchemaNames.Historical)
                    .CommandTimeout(10_000);
                x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
            options.EnableSensitiveDataLogging();
        });

        return services;
    }
}
