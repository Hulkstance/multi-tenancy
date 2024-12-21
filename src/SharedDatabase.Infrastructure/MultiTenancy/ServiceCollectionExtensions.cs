﻿using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharedDatabase.Infrastructure.MultiTenancy;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMultiTenancy(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddDbContext<TenantDbContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                // NOTE: This should be stored in a separate database (should use a different connection string)
                // if we choose the database per tenant approach.
                // Database per tenant means you have a separate database for each application tenant,
                // so tenant metadata cannot be stored in any of these tenant-specific databases. Makes sense?
                var connectionString = configuration.GetConnectionString("Database");

                options.UseSqlServer(connectionString, x =>
                {
                    x.MigrationsHistoryTable("EFMigrations", SchemaNames.Historical)
                        .CommandTimeout(10_000);
                });
            })
            .AddMultiTenant<AppTenantInfo>()
                .WithHeaderStrategy(MultiTenancyConstants.TenantIdHeader)
                .WithEFCoreStore<TenantDbContext, AppTenantInfo>();

        return services;
    }

    public static IApplicationBuilder UseMultiTenancy(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.UseMultiTenant();
    }
}
