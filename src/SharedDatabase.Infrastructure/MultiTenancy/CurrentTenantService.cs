using SharedDatabase.Application.MultiTenancy;

namespace SharedDatabase.Infrastructure.MultiTenancy;

internal class CurrentTenantService(AppTenantInfo currentTenant) : ICurrentTenantService
{
    public string Id => currentTenant.Id!;
    public string Identifier => currentTenant.Identifier!;
}
