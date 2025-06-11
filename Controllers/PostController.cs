using System.Security.Claims;
using KeepTheApex.DTOs;
using KeepTheApex.Hubs;
using KeepTheApex.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace KeepTheApex.Controllers;

[ApiController]
[Route("api/posts")]
[Authorize]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IHubContext<NotificationHub> _hubContext;


    public PostController(IPostService postService, IHubContext<NotificationHub> hubContext)
    {
        _postService = postService;
        _hubContext = hubContext;
    }

    // GET /api/posts/{id}
    [HttpGet("{id}")]
    [AllowAnonymous] 
    public async Task<ActionResult<PostDto>> GetPostById(string id)
    {
        var post = await _postService.GetPostByIdAsync(id);
        if (post == null) return NotFound();
        return Ok(post);
    }

    // POST /api/posts
    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "user";
        var result = await _postService.CreatePostAsync(dto, userId, userRole);
        
        await _hubContext
            .Clients
            .All
            .SendAsync("ReceiveNotification",
                "Your favourite team posted!");
        
        return CreatedAtAction(nameof(GetPostById), new { id = result.Id }, result);
    }

    // POST /api/posts/{id}/like
    [HttpPost("{id}/like")]
    public async Task<IActionResult> LikePost(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _postService.LikePostAsync(id, userId);
        return NoContent();
    }

    // POST /api/posts/{id}/repost
    [HttpPost("{id}/repost")]
    public async Task<IActionResult> RepostPost(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "user";
        await _postService.RepostPostAsync(id, userId, userRole);
        return NoContent();
    }
}