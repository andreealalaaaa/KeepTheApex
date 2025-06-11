using KeepTheApex.DTOs;
using KeepTheApex.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.NotificationHubs;

namespace KeepTheApex.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hub;

    public NotificationService(IHubContext<NotificationHub> hub )
    {
        _hub = hub;
    }

    public async Task SendToTopicAsync(string topic, string title, string body, object data = null)
    {
        var payload = new { title, body, data };
        // Send JSON payload to all connections in the group
        await _hub.Clients.Group(topic)
            .SendAsync("ReceiveNotification", payload);
    }
}