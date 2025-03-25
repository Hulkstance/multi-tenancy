using System.Security.Claims;

namespace SharedDatabase.Infrastructure.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal) => principal.FindFirstValue(ClaimTypes.NameIdentifier);

    public static string? GetTenantId(this ClaimsPrincipal principal) => principal.FindFirstValue(CustomClaimTypes.TenantIdentifier);
}
