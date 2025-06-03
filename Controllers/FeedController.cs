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
    
    // GET /api/feed?filter=driver|team|event
    [HttpGet]
    public async Task<ActionResult<List<PostDto>>> GetFeed([FromQuery] string filter, [FromQuery] string value)
    {
        // Implementation will call _feedService
        return Ok();
    }
}