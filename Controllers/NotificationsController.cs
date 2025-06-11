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
    
    [HttpPost("test")]
    public async Task<IActionResult> Test()//([FromBody] string topic, string title, string body)
    {
        var topic1 = "fp1";
        var title1= "Fastest Lap!";
        var body1 = "Piastri set the fastest lap!";
        
        await _notificationService.SendToTopicAsync(topic1, title1, body1);
        return NoContent();
    }
}