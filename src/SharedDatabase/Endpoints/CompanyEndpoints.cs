using Microsoft.EntityFrameworkCore;
using SharedDatabase.Domain.Entities;
using SharedDatabase.Infrastructure.Persistence;

namespace SharedDatabase.Endpoints;

public static class CompanyEndpoints
{
    public static IEndpointRouteBuilder MapCompanyEndpoints(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapGet("api/companies", async (AppDbContext dbContext, CancellationToken cancellationToken = default) =>
        {
            var companies = await dbContext.Companies
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Results.Ok(companies);
        });

        app.MapGet("api/companies/{id:int}", async (int id, AppDbContext dbContext, CancellationToken cancellationToken = default) =>
        {
            var company = await dbContext.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return company is not null ? Results.Ok(company) : Results.NotFound();
        });

        app.MapPost("api/companies", async (Company company, AppDbContext dbContext, CancellationToken cancellationToken = default) =>
        {
            dbContext.Companies.Add(company);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Results.Created($"api/companies/{company.Id}", company);
        });

        app.MapPut("api/companies/{id:int}", async (int id, Company updatedCompany, AppDbContext dbContext, CancellationToken cancellationToken = default) =>
        {
            var company = await dbContext.Companies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (company is null)
            {
                return Results.NotFound();
            }

            company.Name = updatedCompany.Name;

            await dbContext.SaveChangesAsync(cancellationToken);
            return Results.Ok(company);
        });

        app.MapDelete("api/companies/{id:int}", async (int id, AppDbContext dbContext, CancellationToken cancellationToken = default) =>
        {
            var company = await dbContext.Companies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (company is null)
            {
                return Results.NotFound();
            }

            dbContext.Companies.Remove(company);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        });

        return app;
    }
}
