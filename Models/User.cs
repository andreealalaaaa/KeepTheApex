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
    [JsonProperty("email")]
    public string Email { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public string PasswordHash { get; set; }
    [JsonProperty("role")]
    public string Role { get; set; } = "user";
    [JsonPropertyName("favoriteTeams")]
    public List<string> FavoriteTeams { get; set; }
    [JsonPropertyName("favoriteDrivers")]
    public List<string> FavoriteDrivers { get; set; }
    [JsonPropertyName("repostedPostIds")]
    public List<string> RepostedPostIds { get; set; }
    [JsonPropertyName("likedPostIds")]
    public List<string> LikedPostIds { get; set; }
}