using Microsoft.EntityFrameworkCore;
using SharedDatabase.Domain.Entities;
using SharedDatabase.Infrastructure.Persistence;

namespace SharedDatabase.Endpoints;

public static class SalesEndpoints
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;

    public static IEndpointRouteBuilder MapSalesEndpoints(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapGet("api/sales", async (
            int companyId,
            AppDbContext dbContext,
            int page = DefaultPage,
            int pageSize = DefaultPageSize,
            CancellationToken cancellationToken = default) =>
        {
            var companyExists = await dbContext.Companies
                .AsNoTracking()
                .AnyAsync(x => x.Id == companyId, cancellationToken);

            if (!companyExists)
            {
                return Results.NotFound($"CompanyId '{companyId}' does not exist.");
            }

            var sales = await dbContext.Sales
                .Include(x => x.Company)
                .Where(x => x.CompanyId == companyId)
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return Results.Ok(sales);
        });

        app.MapGet("api/sales/{id:int}", async (int id, AppDbContext dbContext, CancellationToken cancellationToken = default) =>
        {
            var sale = await dbContext.Sales
                .Include(x => x.Company)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return sale is not null ? Results.Ok(sale) : Results.NotFound();
        });

        app.MapPost("api/sales", async (Sale sale, AppDbContext dbContext, CancellationToken cancellationToken = default) =>
        {
            var companyExists = await dbContext.Companies
                .AsNoTracking()
                .AnyAsync(x => x.Id == sale.CompanyId, cancellationToken);

            if (!companyExists)
            {
                return Results.BadRequest($"CompanyId '{sale.CompanyId}' does not exist.");
            }

            dbContext.Sales.Add(sale);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Results.Created($"api/sales/{sale.Id}", sale);
        });

        return app;
    }
}
