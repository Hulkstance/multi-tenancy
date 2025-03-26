using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedDatabase.Application.Common.Interfaces;
using SharedDatabase.Infrastructure.Authentication;
using SharedDatabase.Infrastructure.MultiTenancy;
using SharedDatabase.Infrastructure.Notifications;
using SharedDatabase.Infrastructure.Persistence;

namespace SharedDatabase.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        return services
            .AddPersistence(configuration)
            .AddAuthenticationInternal()
            .AddAuthorizationInternal()
            .AddMultiTenancy(configuration)
            .AddNotifications();
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<AppDbContext>(options =>
        {
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

    private static IServiceCollection AddAuthenticationInternal(this IServiceCollection services)
    {
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName);

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        return services;
    }

    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection AddMultiTenancy(this IServiceCollection services, IConfiguration configuration)
    {
        // If using Database per tenant, this should be in a shared database across tenants, so all tenants can read the metadata from it.
        var connectionString = configuration.GetConnectionString("Database");

        services
            .AddDbContext<TenantDbContext>(options =>
            {
                options.UseSqlServer(connectionString, x =>
                {
                    x.MigrationsHistoryTable("EFMigrations", SchemaNames.Historical)
                        .CommandTimeout(10_000);
                });
            })
            .AddMultiTenant<AppTenantInfo>()
            .WithClaimStrategy(CustomClaimTypes.TenantIdentifier)
            .WithStrategy<SignalRContextItemsStrategy>(ServiceLifetime.Singleton)
            .WithEFCoreStore<TenantDbContext, AppTenantInfo>();

        services.AddScoped<ICurrentTenantService, CurrentTenantService>();

        return services;
    }

    private static IServiceCollection AddNotifications(this IServiceCollection services)
    {
        services.AddSignalR();

        services.AddScoped<INotificationSender, NotificationSender>();

        return services;
    }
}
