namespace KeepTheApex.DTOs;

public class UserDto
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public List<string> FavoriteTeams { get; set; }
    public List<string> FavoriteDrivers { get; set; }
    public List<string> RepostedPostIds { get; set; }
    public List<string> LikedPostIds { get; set; }
}