using Bogus;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedDatabase.Domain.Entities;
using SharedDatabase.Infrastructure.MultiTenancy;
using SharedDatabase.Infrastructure.Persistence;

namespace MigrationService.Host;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await EnsureDatabaseAsync<AppDbContext>(stoppingToken);
            await MigrateDatabaseAsync<AppDbContext>(stoppingToken);

            await EnsureDatabaseAsync<TenantDbContext>(stoppingToken);
            await MigrateDatabaseAsync<TenantDbContext>(stoppingToken);

            await SeedTenantDataAsync(stoppingToken);
            await SeedDataAsync(stoppingToken);
        }
        finally
        {
            hostApplicationLifetime.StopApplication();
        }
    }

    private async Task EnsureDatabaseAsync<TDbContext>(CancellationToken cancellationToken)
        where TDbContext : DbContext
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it does not exist.
            // Do this first so there is then a database to start a transaction against.
            if (!await dbCreator.ExistsAsync(cancellationToken))
            {
                await dbCreator.CreateAsync(cancellationToken);
            }
        });
    }

    private async Task MigrateDatabaseAsync<TDbContext>(CancellationToken cancellationToken)
        where TDbContext : DbContext
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            await dbContext.Database.MigrateAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }

    private async Task SeedTenantDataAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();

        var alreadySeeded = await dbContext.TenantInfo.AsNoTracking().AnyAsync(cancellationToken);
        if (alreadySeeded)
        {
            return;
        }

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            var faker = new Faker();
            List<AppTenantInfo> tenants = [];

            for (var i = 1; i <= 10; i++)
            {
                tenants.Add(new AppTenantInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    Identifier = $"tenant{i}",
                    Name = faker.Company.CompanyName()
                });
            }

            await dbContext.TenantInfo.AddRangeAsync(tenants, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });
    }

    private async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var tenantStore = scope.ServiceProvider.GetRequiredService<IMultiTenantStore<AppTenantInfo>>();
        var tenants = await tenantStore.GetAllAsync();

        foreach (var tenant in tenants)
        {
            await using var tenantScope = serviceProvider.CreateAsyncScope();

            var multiTenantContextSetter = tenantScope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
            var multiTenantContext = new MultiTenantContext<AppTenantInfo> { TenantInfo = tenant };
            multiTenantContextSetter.MultiTenantContext = multiTenantContext;

            await using var dbContext = tenantScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var alreadySeeded = await dbContext.Companies.AsNoTracking().AnyAsync(cancellationToken);
            if (alreadySeeded)
            {
                return;
            }

            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                // Seed the database
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                var faker = new Faker();
                var companies = await SeedCompaniesAsync(faker, dbContext, cancellationToken);
                await SeedSalesAsync(faker, dbContext, companies, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            });
        }
    }

    private static async Task<List<Company>> SeedCompaniesAsync(Faker faker, AppDbContext dbContext, CancellationToken cancellationToken)
    {
        List<Company> companies = [];
        for (var i = 0; i < faker.Random.Int(3, 5); i++)
        {
            companies.Add(new Company
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CompanyName()
            });
        }

        await dbContext.Companies.AddRangeAsync(companies, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return companies;
    }

    private static async Task SeedSalesAsync(Faker faker, AppDbContext dbContext, List<Company> companies, CancellationToken cancellationToken)
    {
        List<Sale> sales = [];
        foreach (var company in companies)
        {
            for (var i = 0; i < faker.Random.Int(2, 4); i++)
            {
                sales.Add(new Sale
                {
                    Id = Guid.NewGuid(),
                    Amount = Math.Round(faker.Random.Decimal(500, 5000), 2),
                    CreatedAt = faker.Date.Between(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow),
                    CompanyId = company.Id
                });
            }
        }

        await dbContext.Sales.AddRangeAsync(sales, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
