using System.Security.Claims;
using System.Text;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedDatabase.Application.Common.Exceptions;
using SharedDatabase.Infrastructure.MultiTenancy;

namespace SharedDatabase.Infrastructure.Authentication;

public class ConfigureJwtBearerOptions(IOptions<JwtOptions> jwtOptions) : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(JwtBearerOptions options) =>
        Configure(JwtBearerDefaults.AuthenticationScheme, options);

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        var key = Encoding.UTF8.GetBytes(jwtOptions.Value.Key);
        var issuer = jwtOptions.Value.Issuer;
        var audience = jwtOptions.Value.Audience;

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.Zero // We don't tolerate any token where the expiration time is in the past
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var identity = context.Principal?.Identities.FirstOrDefault();
                var tenantClaim = identity?.FindFirst(CustomClaimTypes.TenantIdentifier);
                var userIdClaim = identity?.FindFirst(ClaimTypes.NameIdentifier);

                if (identity == null || tenantClaim == null || userIdClaim == null)
                {
                    throw new UnauthorizedException("Authentication Failed.");
                }

                var tenantStore = context.HttpContext.RequestServices.GetRequiredService<IMultiTenantStore<AppTenantInfo>>();
                var tenant = await tenantStore.TryGetByIdentifierAsync(tenantClaim.Value);

                if (tenant == null)
                {
                    throw new UnauthorizedException("Authentication Failed.");
                }

                context.HttpContext.SetTenantInfo(tenant, true);
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnForbidden = _ => throw new ForbiddenException("You are not authorized to access this resource.")
        };
    }
}
