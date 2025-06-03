using KeepTheApex.DTOs;
using KeepTheApex.Services;
using Microsoft.AspNetCore.Mvc;

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
}