using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;
using Microsoft.EntityFrameworkCore;

namespace SharedDatabase.Infrastructure.MultiTenancy;

// This database context is not itself multi-tenant,
// but rather it globally contains the details of each tenant.
// It will often be a standalone database separate from any tenant database(s)
// and will have its own connection string.
public class TenantDbContext(DbContextOptions<TenantDbContext> options) : EFCoreStoreDbContext<AppTenantInfo>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppTenantInfo>().ToTable("Tenants", SchemaNames.MultiTenancy);
    }
}
