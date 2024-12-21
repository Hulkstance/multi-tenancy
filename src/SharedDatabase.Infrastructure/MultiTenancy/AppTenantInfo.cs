using Finbuckle.MultiTenant.Abstractions;

namespace SharedDatabase.Infrastructure.MultiTenancy;

public class AppTenantInfo : ITenantInfo
{
    /// <summary>
    /// The actual TenantId, which is also used in the TenantId shadow property on the multitenant entities.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// The identifier that is used in headers/routes/querystrings. This is set to the same as Id to avoid confusion.
    /// </summary>
    public string? Identifier { get; set; }

    public string? Name { get; set; }
}
