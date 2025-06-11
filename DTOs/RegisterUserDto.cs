namespace KeepTheApex.DTOs;

public class RegisterUserDto
{
    public string FullName { get; set; } = null!;
    public string Email    { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role     { get; set; } = "user";
}