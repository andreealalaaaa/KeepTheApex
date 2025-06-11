using KeepTheApex.DTOs;
using KeepTheApex.Models;
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
    
    [HttpPost]
    public async Task<IActionResult> Test([FromBody] NotificationDto notificationDto)
    {
        if (notificationDto == null || string.IsNullOrEmpty(notificationDto.Topic) ||
            string.IsNullOrEmpty(notificationDto.Title))
        {
            return BadRequest("Invalid notification data");
        }

        await _notificationService.SendToTopicAsync(notificationDto.Topic, notificationDto.Title, notificationDto.Body);
        return NoContent();
    }
}