using Microsoft.AspNetCore.SignalR;

namespace KeepTheApex.Hubs;

public class NotificationHub: Hub   
{
    // Client calls this to join a “topic” group
    public Task Subscribe(string topic) =>
        Groups.AddToGroupAsync(Context.ConnectionId, topic);

    public Task Unsubscribe(string topic) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, topic);
}