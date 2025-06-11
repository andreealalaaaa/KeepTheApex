using Microsoft.AspNetCore.Mvc;
using KeepTheApex.DTOs;
using KeepTheApex.Services;

namespace KeepTheApex.Controllers;

[ApiController]
[Route("api/feed")]
public class FeedController: ControllerBase
{
    private readonly IFeedService _feedService;

    public FeedController(IFeedService feedService)
    {
        _feedService = feedService;
    }
    
    // GET /api/feed
    [HttpGet]
    public async Task<ActionResult<List<PostDto>>> GetFeed(
        [FromQuery] string? filter = null,
        [FromQuery] string? value  = null)
    {
        var feed = await _feedService.GetFeedAsync(filter, value);
        return Ok(feed);
    }
}