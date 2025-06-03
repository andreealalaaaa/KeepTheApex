using KeepTheApex.DTOs;
using KeepTheApex.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeepTheApex.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController: ControllerBase
{
    private readonly INotificationService _notificationService;
    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    // POST /api/notifications/trigger
    [HttpPost("trigger")]
    public async Task<IActionResult> TriggerNotification([FromBody] NotificationTriggerDto dto)
    {
        // Implementation will call _notificationService
        return NoContent();
    }
}