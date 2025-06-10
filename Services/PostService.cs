using KeepTheApex.DTOs;
using KeepTheApex.Models;
using Microsoft.Azure.Cosmos;

namespace KeepTheApex.Services;

public class PostService : IPostService
{
    private readonly Container _container;

    public PostService(CosmosClient cosmosClient)
    {
        var databaseId = "KeepTheApexDb";
        var containerId = "Posts";
        var partitionKeyPath = "/id";

        try
        {
            var database = cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            _container = database.Database.CreateContainerIfNotExistsAsync(
                id: containerId,
                partitionKeyPath: partitionKeyPath,
                throughput: 400
            ).Result.Container;

            Console.WriteLine("Connected to or created Cosmos DB container: Users");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to connect or create Cosmos DB container: " + ex.Message);
            throw;
        }
    }

    public async Task<PostDto> CreatePostAsync(CreatePostDto dto, string authorId, string authorRole, string? repostOfId = null)
    {
        var post = new Post
        {
            Id = Guid.NewGuid().ToString(),
            AuthorId = authorId,
            AuthorRole = authorRole,
            Content = dto.Content,
            MediaUrl = dto.MediaUrl,
            Timestamp = DateTime.UtcNow,
            Likes = new List<string>(),
            Reposts = new List<string>(),
            RepostOfId = repostOfId
        };

        await _container.CreateItemAsync(post, new PartitionKey(post.Id));

        return ToDto(post);
    }

    public async Task<PostDto?> GetPostByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Post>(id, new PartitionKey(id));
            return ToDto(response.Resource);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task LikePostAsync(string postId, string userId)
    {
        var post = await _container.ReadItemAsync<Post>(postId, new PartitionKey(postId));
        if (!post.Resource.Likes.Contains(userId))
        {
            post.Resource.Likes.Add(userId);
            await _container.ReplaceItemAsync(post.Resource, postId, new PartitionKey(postId));
        }
    }

    public async Task RepostPostAsync(string postId, string userId, string userRole)
    {
        var parentPost = await _container.ReadItemAsync<Post>(postId, new PartitionKey(postId));
        if (!parentPost.Resource.Reposts.Contains(userId))
        {
            parentPost.Resource.Reposts.Add(userId);
            await _container.ReplaceItemAsync(parentPost.Resource, postId, new PartitionKey(postId));
        }

        var repostDto = new CreatePostDto
        {
            Content = parentPost.Resource.Content,
            MediaUrl = parentPost.Resource.MediaUrl
        };

        await CreatePostAsync(repostDto, userId, userRole, postId);
    }

    private PostDto ToDto(Post post) => new()
    {
        Id = post.Id,
        AuthorId = post.AuthorId,
        AuthorRole = post.AuthorRole,
        Content = post.Content,
        MediaUrl = post.MediaUrl,
        Timestamp = post.Timestamp,
        Likes = post.Likes,
        Reposts = post.Reposts,
        RepostOfId = post.RepostOfId
    };
}
