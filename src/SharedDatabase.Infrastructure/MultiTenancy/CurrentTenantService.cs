using Finbuckle.MultiTenant.Abstractions;
using SharedDatabase.Application.MultiTenancy;

namespace SharedDatabase.Infrastructure.MultiTenancy;

internal class CurrentTenantService(IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor) : ICurrentTenantService
{
    public string Id => multiTenantContextAccessor.MultiTenantContext.TenantInfo?.Id!;
    public string Identifier => multiTenantContextAccessor.MultiTenantContext.TenantInfo?.Identifier!;
}
