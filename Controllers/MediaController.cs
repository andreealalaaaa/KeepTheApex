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
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<string>> UploadMedia([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }
        
        var url = await _mediaService.UploadMediaAsync(file);
        
        return Ok(url);
    }
    
    // GET /api/media/{fileName}
    [HttpGet("{fileName}")]
    public async Task<IActionResult> Download([FromRoute] string fileName)
    {
        var stream = await _mediaService.GetMediaAsync(fileName);
        if (stream is null) return NotFound();

        return File(stream, "application/octet-stream", fileName);
    }
    
    // GET /api/media/download?url={blobUrl}
    [HttpGet("download")]
    public async Task<IActionResult> DownloadByUrl([FromQuery] string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return BadRequest("Invalid URL");

        var stream = await _mediaService.GetMediaByUrlAsync(url);
        if (stream is null)
            return NotFound();

        var name = Path.GetFileName(uri.LocalPath);
        return File(stream, "application/octet-stream", name);
    }
}