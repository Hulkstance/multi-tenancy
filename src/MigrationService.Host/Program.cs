using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MigrationService.Host;
using SharedDatabase.Infrastructure.Authentication;
using SharedDatabase.Infrastructure.MultiTenancy;
using SharedDatabase.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString, x =>
    {
        x.MigrationsHistoryTable("EFMigrations", SchemaNames.Historical)
            .CommandTimeout(10_000);
    });
});

builder.Services
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
    .WithEFCoreStore<TenantDbContext, AppTenantInfo>();

var host = builder.Build();

host.Run();
