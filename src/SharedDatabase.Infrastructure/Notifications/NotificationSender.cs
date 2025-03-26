using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.SignalR;
using SharedDatabase.Application.Common.Interfaces;
using SharedDatabase.Infrastructure.MultiTenancy;

namespace SharedDatabase.Infrastructure.Notifications;

public class NotificationSender(
    IHubContext<NotificationHub> hubContext,
    IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor)
    : INotificationSender
{
    private readonly AppTenantInfo? _currentTenant = multiTenantContextAccessor.MultiTenantContext.TenantInfo;

    public Task BroadcastAsync<T>(string method, T message, CancellationToken cancellationToken = default) =>
        hubContext.Clients.All
            .SendAsync(method, message, cancellationToken);

    public Task BroadcastAsync<T>(string method, T message, IEnumerable<string> excludedConnectionIds, CancellationToken cancellationToken = default) =>
        hubContext.Clients.AllExcept(excludedConnectionIds)
            .SendAsync(method, message, cancellationToken);

    public Task SendToAllAsync<T>(string method, T message, CancellationToken cancellationToken = default)
    {
        if (_currentTenant is null)
        {
            return Task.CompletedTask;
        }

        return hubContext.Clients.Group($"GroupTenant-{_currentTenant.Identifier}")
            .SendAsync(method, message, cancellationToken);
    }

    public Task SendToAllAsync<T>(string method, T message, IEnumerable<string> excludedConnectionIds, CancellationToken cancellationToken = default)
    {
        if (_currentTenant is null)
        {
            return Task.CompletedTask;
        }

        return hubContext.Clients.GroupExcept($"GroupTenant-{_currentTenant.Identifier}", excludedConnectionIds)
            .SendAsync(method, message, cancellationToken);
    }

    public Task SendToGroupAsync<T>(string method, T message, string group, CancellationToken cancellationToken = default) =>
        hubContext.Clients.Group(group)
            .SendAsync(method, message, cancellationToken);

    public Task SendToGroupAsync<T>(string method, T message, string group, IEnumerable<string> excludedConnectionIds, CancellationToken cancellationToken = default) =>
        hubContext.Clients.GroupExcept(group, excludedConnectionIds)
            .SendAsync(method, message, cancellationToken);

    public Task SendToGroupsAsync<T>(string method, T message, IEnumerable<string> groupNames, CancellationToken cancellationToken = default) =>
        hubContext.Clients.Groups(groupNames)
            .SendAsync(method, message, cancellationToken);

    public Task SendToUserAsync<T>(string method, T message, string userId, CancellationToken cancellationToken = default) =>
        hubContext.Clients.User(userId)
            .SendAsync(method, message, cancellationToken);

    public Task SendToUsersAsync<T>(string method, T message, IEnumerable<string> userIds, CancellationToken cancellationToken = default) =>
        hubContext.Clients.Users(userIds)
            .SendAsync(method, message, cancellationToken);
}
