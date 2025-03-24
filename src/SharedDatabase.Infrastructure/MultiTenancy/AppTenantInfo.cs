using Finbuckle.MultiTenant.Abstractions;

namespace SharedDatabase.Infrastructure.MultiTenancy;

public class AppTenantInfo : ITenantInfo
{
    /// <inheritdoc />
    public string? Id { get; set; }

    /// <inheritdoc />
    public string? Identifier { get; set; }

    /// <inheritdoc />
    public string? Name { get; set; }
}
