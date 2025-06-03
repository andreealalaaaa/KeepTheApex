using Azure.Storage.Blobs;
using KeepTheApex.DTOs;
using KeepTheApex.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.NotificationHubs;
using UserModel = KeepTheApex.Models.User;
using PostModel = KeepTheApex.Models.Post;

namespace KeepTheApex.Services;

public class UserService : IUserService
{
    private readonly Container _container;
    public UserService(CosmosClient cosmosClient)
    {
        _container = cosmosClient.GetContainer("KeepTheApexDb", "Users");
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<UserModel>(id, new PartitionKey(id));
            var user = response.Resource;
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                FavoriteTeams = user.FavoriteTeams,
                FavoriteDrivers = user.FavoriteDrivers,
                RepostedPostIds = user.RepostedPostIds,
                LikedPostIds = user.LikedPostIds
            };
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task UpdateFavoritesAsync(string id, List<string> teams, List<string> drivers)
    {
        var response = await _container.ReadItemAsync<UserModel>(id, new PartitionKey(id));
        var user = response.Resource;
        user.FavoriteTeams = teams;
        user.FavoriteDrivers = drivers;
        await _container.ReplaceItemAsync(user, id, new PartitionKey(id));
    }
}

public class PostService : IPostService
{
    private readonly Container _container;
    public PostService(CosmosClient cosmosClient)
    {
        _container = cosmosClient.GetContainer("KeepTheApexDb", "Posts");
    }

    public async Task<PostDto> CreatePostAsync(string authorId, string authorRole, string content, string? mediaUrl)
    {
        var post = new PostModel
        {
            Id = Guid.NewGuid().ToString(),
            AuthorId = authorId,
            AuthorRole = authorRole,
            Content = content,
            MediaUrl = mediaUrl ?? string.Empty,
            Timestamp = DateTime.UtcNow,
            Likes = new List<string>(),
            Reposts = new List<string>()
        };
        await _container.CreateItemAsync(post, new PartitionKey(post.Id));
        return new PostDto
        {
            Id = post.Id,
            AuthorId = post.AuthorId,
            AuthorRole = post.AuthorRole,
            Content = post.Content,
            MediaUrl = post.MediaUrl ?? string.Empty,
            Timestamp = post.Timestamp,
            Likes = post.Likes,
            Reposts = post.Reposts
        };
    }

    public async Task LikePostAsync(string postId, string userId)
    {
        var response = await _container.ReadItemAsync<Post>(postId, new PartitionKey(postId));
        var post = response.Resource;
        if (!post.Likes.Contains(userId))
        {
            post.Likes.Add(userId);
            await _container.ReplaceItemAsync(post, postId, new PartitionKey(postId));
        }
    }

    public async Task RepostPostAsync(string postId, string userId)
    {
        var response = await _container.ReadItemAsync<Post>(postId, new PartitionKey(postId));
        var post = response.Resource;
        if (!post.Reposts.Contains(userId))
        {
            post.Reposts.Add(userId);
            await _container.ReplaceItemAsync(post, postId, new PartitionKey(postId));
        }
    }
    
    public async Task<PostDto?> GetPostByIdAsync(string postId)
    {
        try
        {
            var response = await _container.ReadItemAsync<PostModel>(postId, new PartitionKey(postId));
            var post = response.Resource;
            return new PostDto
            {
                Id = post.Id,
                AuthorId = post.AuthorId,
                AuthorRole = post.AuthorRole,
                Content = post.Content,
                MediaUrl = post.MediaUrl ?? string.Empty,
                Timestamp = post.Timestamp,
                Likes = post.Likes,
                Reposts = post.Reposts
            };
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}

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
