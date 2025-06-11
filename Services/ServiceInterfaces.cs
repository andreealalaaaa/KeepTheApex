using KeepTheApex.DTOs;
using KeepTheApex.Models;

namespace KeepTheApex.Services;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string id);
    Task UpdateFavoritesAsync(string id, List<string> teams, List<string> drivers);
    Task CreateUserAsync(User user);
    Task<List<User>> GetAllUsersAsync();
    Task<UserDto>         RegisterUserAsync(RegisterUserDto dto);
    Task<UserDto?>        LoginAsync(LoginDto dto);
}

public interface IPostService
{
    Task<PostDto> CreatePostAsync(CreatePostDto dto, string authorId, string authorRole, string? repostOfId = null);
    Task<PostDto?> GetPostByIdAsync(string id);
    Task LikePostAsync(string postId, string userId);
    Task RepostPostAsync(string postId, string userId, string userRole);
}


public interface IMediaService
{
    Task<string> UploadMediaAsync(IFormFile file);
    Task<Stream?> GetMediaAsync(string fileName);
    Task<Stream?> GetMediaByUrlAsync(string url);
}

public interface INotificationService
{
    Task SendToTopicAsync(string topic, string title, string body, object data = null);
}

public interface IFeedService
{
    Task<List<PostDto>> GetFeedAsync(string? filterType = null, string? filterValue = null);
}

