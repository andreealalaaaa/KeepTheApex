namespace KeepTheApex.Models;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public List<string> FavoriteTeams { get; set; }
    public List<string> FavoriteDrivers { get; set; }
    public List<string> RepostedPostIds { get; set; }
    public List<string> LikedPostIds { get; set; }
}