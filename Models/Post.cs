using Newtonsoft.Json;

namespace KeepTheApex.Models;

public class Post
{
    [JsonProperty("id")]
    public string Id { get; set; }
    public string AuthorId { get; set; }
    public string AuthorRole { get; set; }
    public string Content { get; set; }
    public string MediaUrl { get; set; }
    public DateTime Timestamp { get; set; }
    public List<string> Likes { get; set; }
    public List<string> Reposts { get; set; }
    public string? RepostOfId { get; set; }
}