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
    public void Configure(JwtBearerOptions options) => Configure(JwtBearerDefaults.AuthenticationScheme, options);

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        var key = Encoding.UTF8.GetBytes(jwtOptions.Value.Key);

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Value.Issuer,
            ValidAudience = jwtOptions.Value.Audience,
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.Zero // We don't tolerate any token where the expiration time is in the past
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var tenantId = context.Principal?.GetTenantId();
                if (tenantId is null)
                {
                    throw new UnauthorizedException("Tenant ID is missing from the JWT token");
                }

                var tenantStore = context.HttpContext.RequestServices.GetRequiredService<IMultiTenantStore<AppTenantInfo>>();
                var tenantInfo = await tenantStore.TryGetByIdentifierAsync(tenantId);
                if (tenantInfo is null)
                {
                    throw new UnauthorizedException($"No tenant found with ID '{tenantId}' in the tenant store");
                }

                context.HttpContext.SetTenantInfo(tenantInfo, true);
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnForbidden = _ => throw new ForbiddenException("You are not authorized to access this resource")
        };
    }
}
