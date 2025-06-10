using Azure.Storage.Blobs;
using KeepTheApex.DTOs;
using KeepTheApex.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.NotificationHubs;
using UserModel = KeepTheApex.Models.User;
using PostModel = KeepTheApex.Models.Post;

namespace KeepTheApex.Services;

public class FeedService : IFeedService
{
    private readonly Container _postContainer;
    public FeedService(CosmosClient cosmosClient)
    {
        _postContainer = cosmosClient.GetContainer("KeepTheApexDb", "Posts");
    }

    public async Task<List<PostDto>> GetFeedAsync(string? filterType = null, string? filterValue = null)
    {
        var query = "SELECT * FROM c";
        if (!string.IsNullOrEmpty(filterType) && !string.IsNullOrEmpty(filterValue))
        {
            if (filterType == "team")
                query = $"SELECT * FROM c WHERE ARRAY_CONTAINS(c.teamsMentioned, '{filterValue}')";
            else if (filterType == "driver")
                query = $"SELECT * FROM c WHERE ARRAY_CONTAINS(c.driversMentioned, '{filterValue}')";
            else if (filterType == "event")
                query = $"SELECT * FROM c WHERE c.event = '{filterValue}'";
        }
        var iterator = _postContainer.GetItemQueryIterator<PostModel>(query);
        var results = new List<PostDto>();
        while (iterator.HasMoreResults)
        {
            foreach (var post in await iterator.ReadNextAsync())
            {
                results.Add(new PostDto
                {
                    Id = post.Id,
                    AuthorId = post.AuthorId,
                    AuthorRole = post.AuthorRole,
                    Content = post.Content,
                    MediaUrl = post.MediaUrl ?? string.Empty,
                    Timestamp = post.Timestamp,
                    Likes = post.Likes,
                    Reposts = post.Reposts
                });
            }
        }
        return results.OrderByDescending(p => p.Timestamp).ToList();
    }
}

public class MediaService : IMediaService
{
    private readonly BlobContainerClient _containerClient;
    public MediaService(BlobServiceClient blobServiceClient)
    {
        _containerClient = blobServiceClient.GetBlobContainerClient("media");
    }

    public async Task<string> UploadMediaAsync(IFormFile file)
    {
        var blobClient = _containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(file.FileName));
        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, true);
        }
        return blobClient.Uri.ToString();
    }
}

public class NotificationService : INotificationService
{
    private readonly NotificationHubClient _hubClient;
    public NotificationService(NotificationHubClient hubClient)
    {
        _hubClient = hubClient;
    }

    public async Task TriggerNotificationAsync(string teamId, string driverId, string postId)
    {
        // Example: send a template notification to all users following the team/driver
        var payload = $"{{ \"teamId\": \"{teamId}\", \"driverId\": \"{driverId}\", \"postId\": \"{postId}\" }}";
        // In production, use tags for targeting followers
        await _hubClient.SendFcmNativeNotificationAsync(payload);
    }
}
