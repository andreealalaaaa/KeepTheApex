using KeepTheApex.DTOs;
using KeepTheApex.Services;
using Microsoft.AspNetCore.Mvc;
using User = KeepTheApex.Models.User;

namespace KeepTheApex.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController: ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET /api/users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    // PUT /api/users/{id}/favorites
    [HttpPut("{id}/favorites")]
    public async Task<IActionResult> UpdateFavorites(string id, [FromBody] UpdateFavoritesDto dto)
    {
        await _userService.UpdateFavoritesAsync(id, dto.FavoriteTeams, dto.FavoriteDrivers);
        return NoContent();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserDto newUser)
    {
        var user = new User
        {
            Id = newUser.UserId,
            UserId = newUser.UserId,
            Username = newUser.Username,
            Role = newUser.Role,
            FavoriteTeams = newUser.FavoriteTeams ,
            FavoriteDrivers = newUser.FavoriteDrivers,
            RepostedPostIds = newUser.RepostedPostIds,
            LikedPostIds = newUser.LikedPostIds
        };

        try
        {
            await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error creating user: " + ex.Message);
            return StatusCode(500, "Failed to create user.");
        }
    }
}