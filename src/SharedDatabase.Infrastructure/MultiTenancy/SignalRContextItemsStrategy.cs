using Finbuckle.MultiTenant.Abstractions;
using SharedKernel.MultiTenancy;

namespace SharedDatabase.Infrastructure.MultiTenancy;

/// <summary>
/// A tenant resolution strategy for SignalR Hub connections that extracts
/// the tenant identifier from the SignalR Context.Items dictionary.
/// </summary>
public class SignalRContextItemsStrategy : IMultiTenantStrategy
{
    public Task<string?> GetIdentifierAsync(object context)
    {
        // SignalR hub's Context.Items is an IDictionary<object, object?>
        if (context is IDictionary<object, object?> items &&
            items.TryGetValue(MultiTenancyConstants.TenantIdKey, out var tenantIdObj) &&
            tenantIdObj is string tenantId)
        {
            return Task.FromResult<string?>(tenantId);
        }

        return Task.FromResult<string?>(null);
    }
}
