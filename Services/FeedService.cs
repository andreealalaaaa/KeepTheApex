using KeepTheApex.DTOs;
using Microsoft.Azure.Cosmos;
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
                query = $"SELECT * FROM c WHERE ARRAY_CONTAINS(c.Content, '{filterValue}')";
            else if (filterType == "driver")
                query = $"SELECT * FROM c WHERE ARRAY_CONTAINS(c.Content, '{filterValue}')";
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