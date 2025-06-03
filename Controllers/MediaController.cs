using KeepTheApex.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeepTheApex.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController: ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    // POST /api/media/upload
    [HttpPost("upload")]
    public async Task<ActionResult<string>> UploadMedia([FromForm] IFormFile file)
    {
        // Implementation will call _mediaService
        return Ok();
    }
}