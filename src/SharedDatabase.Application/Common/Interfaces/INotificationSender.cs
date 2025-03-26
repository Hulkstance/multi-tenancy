namespace SharedDatabase.Application.Common.Interfaces;

public interface INotificationSender
{
    Task BroadcastAsync<T>(string method, T message, CancellationToken cancellationToken = default);

    Task BroadcastAsync<T>(string method, T message, IEnumerable<string> excludedConnectionIds, CancellationToken cancellationToken = default);

    Task SendToAllAsync<T>(string method, T message, CancellationToken cancellationToken = default);

    Task SendToAllAsync<T>(string method, T message, IEnumerable<string> excludedConnectionIds, CancellationToken cancellationToken = default);

    Task SendToGroupAsync<T>(string method, T message, string group, CancellationToken cancellationToken = default);

    Task SendToGroupAsync<T>(string method, T message, string group, IEnumerable<string> excludedConnectionIds, CancellationToken cancellationToken = default);

    Task SendToGroupsAsync<T>(string method, T message, IEnumerable<string> groupNames, CancellationToken cancellationToken = default);

    Task SendToUserAsync<T>(string method, T message, string userId, CancellationToken cancellationToken = default);

    Task SendToUsersAsync<T>(string method, T message, IEnumerable<string> userIds, CancellationToken cancellationToken = default);
}
