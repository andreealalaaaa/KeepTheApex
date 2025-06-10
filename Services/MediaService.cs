using System.Text.RegularExpressions;
using Azure.Storage.Blobs;

namespace KeepTheApex.Services;

public class MediaService: IMediaService
{
    private readonly BlobContainerClient _container;
    
    public MediaService(BlobServiceClient blobServiceClient)
    { 
        _container = blobServiceClient.GetBlobContainerClient("media");
    }
    
    public async Task<string> UploadMediaAsync(IFormFile file) 
    {
        var originalFileName = Path.GetFileName(file.FileName);
        
        var ext  = Path.GetExtension(originalFileName);  
        var name = Path.GetFileNameWithoutExtension(originalFileName);
        
        var slug = Regex.Replace(name.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');

        var blobName = $"{slug}-{Guid.NewGuid()}{ext}";

        var blobClient = _container.GetBlobClient(blobName);
        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, true);
        }
        
        return blobClient.Uri.ToString();
    }
    
    
    public async Task<Stream?> GetMediaAsync(string fileName)
    {
        var blobClient = _container.GetBlobClient(fileName);
        if (!await blobClient.ExistsAsync()) return null;

        var download = await blobClient.DownloadStreamingAsync();
        return download.Value.Content;
    }
    
    public async Task<Stream?> GetMediaByUrlAsync(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return null;

        var blolbName = Path.GetFileName(uri.LocalPath);
        if (string.IsNullOrEmpty(blolbName))
            return null;

        return await GetMediaAsync(blolbName);
    }
}