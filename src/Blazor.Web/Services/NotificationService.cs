using Microsoft.AspNetCore.SignalR.Client;

namespace Blazor.Web.Services;

public class NotificationService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly string _hubUrl = configuration["SignalR:HubUrl"]!;
    private readonly string _identityUrl = configuration["Identity:Url"]!;
    private bool _started;

    public event Action<string>? OnNotificationReceived;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public async Task StartAsync()
    {
        if (_started)
        {
            return;
        }

        var token = await GetAccessTokenAsync();

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token)!;
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string>("ReceiveNotification", message =>
        {
            OnNotificationReceived?.Invoke(message);
        });

        await _hubConnection.StartAsync();
        _started = true;
    }

    public async Task SendNotificationAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.SendAsync("SendNotification");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

    private async Task<string> GetAccessTokenAsync()
    {
        using var client = httpClientFactory.CreateClient();

        var tokenRequest = new
        {
            userId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            email = "test@example.com",
            customClaims = new Dictionary<string, string>
            {
                ["http://schemas.microsoft.com/identity/claims/tenantid"] = "tenant1"
            }
        };

        var response = await client.PostAsJsonAsync($"{_identityUrl}/token", tokenRequest);
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadAsStringAsync();
        return token;
    }
}
