using KeepTheApex.DTOs;
using KeepTheApex.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeepTheApex.Controllers;

[ApiController]
[Route("api/posts")]
public class PostController: ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    // GET /api/posts/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetPostById(string id)
    {
        return Ok();
    }

    // POST /api/posts
    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto dto)
    {
        // Implementation will call _postService
        return Ok();
    }

    // POST /api/posts/{id}/like
    [HttpPost("{id}/like")]
    public async Task<IActionResult> LikePost(string id)
    {
        // Implementation will call _postService
        return NoContent();
    }

    // POST /api/posts/{id}/repost
    [HttpPost("{id}/repost")]
    public async Task<IActionResult> RepostPost(string id)
    {
        // Implementation will call _postService
        return NoContent();
    }
}