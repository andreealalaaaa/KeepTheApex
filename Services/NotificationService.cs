using KeepTheApex.DTOs;
using Microsoft.Azure.NotificationHubs;

namespace KeepTheApex.Services;

public class NotificationService
{
    private readonly NotificationHubClient _hubClient;
    public NotificationService(NotificationHubClient hubClient)
    {
        _hubClient = hubClient;
    }

    public async Task TriggerNotificationAsync(NotificationTriggerDto dto)
    {
        // Example: send a template notification to all users following the team/driver
        var payload = $"{{ \"teamId\": \"{dto.TeamId}\", \"driverId\": \"{dto.DriverId}\", \"postId\": \"{dto.PostId}\" }}";
        
        // In production, use tags for targeting followers
        await _hubClient.SendFcmNativeNotificationAsync(payload);
    }
}