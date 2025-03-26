using System.Text.Json;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedDatabase.Application.Common.Interfaces;
using SharedDatabase.Infrastructure.Authentication;
using SharedDatabase.Infrastructure.MultiTenancy;
using SharedDatabase.Infrastructure.Persistence;
using SharedKernel.MultiTenancy;

namespace SharedDatabase.Infrastructure.Notifications;

[Authorize]
public class NotificationHub(
    ILogger<NotificationHub> logger,
    IServiceProvider serviceProvider,
    INotificationSender notificationSender)
    : Hub
{
    public override async Task OnConnectedAsync()
    {
        var currentTenantId = Context.User?.GetTenantId();
        if (currentTenantId is null)
        {
            logger.LogWarning("A client with ConnectionId {ConnectionId} tried to connect without a current tenant set. Aborting the connection.", Context.ConnectionId);
            Context.Abort();
            return;
        }

        Context.Items[MultiTenancyConstants.TenantIdKey] = currentTenantId;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"GroupTenant-{currentTenantId}");

        await base.OnConnectedAsync();

        logger.LogInformation("A client connected with ConnectionId {ConnectionId}", Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.Items[MultiTenancyConstants.TenantIdKey] is string tenantId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"GroupTenant-{tenantId}");
        }

        await base.OnDisconnectedAsync(exception);

        logger.LogInformation("A client disconnected with ConnectionId: {ConnectionId}", Context.ConnectionId);
    }

    public async Task SendNotification()
    {
        // https://github.com/Finbuckle/Finbuckle.MultiTenant/issues/963
        await using var tenantScope = serviceProvider.CreateAsyncScope();

        var tenantResolver = tenantScope.ServiceProvider.GetRequiredService<ITenantResolver<AppTenantInfo>>();
        var resolveResult = await tenantResolver.ResolveAsync(Context.Items);
        if (resolveResult.TenantInfo is null)
        {
            logger.LogWarning("Unable to resolve tenant for connection {ConnectionId}", Context.ConnectionId);
            return;
        }

        var multiTenantContextSetter = tenantScope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
        var multiTenantContext = new MultiTenantContext<AppTenantInfo> { TenantInfo = resolveResult.TenantInfo };
        multiTenantContextSetter.MultiTenantContext = multiTenantContext;

        await using var dbContext = tenantScope.ServiceProvider.GetRequiredService<AppDbContext>();

        var companies = await dbContext.Companies
            .AsNoTracking()
            .ToListAsync();

        var companiesJson = JsonSerializer.Serialize(companies);
        await notificationSender.SendToAllAsync("ReceiveNotification", companiesJson);
    }
}
