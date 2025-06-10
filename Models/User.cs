using System.Text.Json.Serialization;
using Newtonsoft.Json;
namespace KeepTheApex.Models;

public class User
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("userId")]
    public string UserId { get; set; }
    [JsonProperty("username")]
    public string Username { get; set; }
    [JsonProperty("role")]
    public string Role { get; set; }
    [JsonPropertyName("favoriteTeams")]
    public List<string> FavoriteTeams { get; set; }
    [JsonPropertyName("favoriteDrivers")]
    public List<string> FavoriteDrivers { get; set; }
    [JsonPropertyName("repostedPostIds")]
    public List<string> RepostedPostIds { get; set; }
    [JsonPropertyName("likedPostIds")]
    public List<string> LikedPostIds { get; set; }
}