using Microsoft.AspNetCore.Mvc;

namespace KeepTheApex.DTOs;

public class UpdateFavoritesDto
{
    public List<string> FavoriteTeams { get; set; } = new List<string>();
    public List<string> FavoriteDrivers { get; set; }   = new List<string>();
}