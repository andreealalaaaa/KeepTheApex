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
        if(dto == null || (string.IsNullOrEmpty(dto.TeamId) && string.IsNullOrEmpty(dto.DriverId)))
        {
            return BadRequest("Invalid notification trigger data.");
        }
        
        await _notificationService.TriggerNotificationAsync(dto);
        return NoContent();
    }
}