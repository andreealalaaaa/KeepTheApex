using KeepTheApex.DTOs;
using KeepTheApex.Models;

namespace KeepTheApex.Services;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string id);
    Task UpdateFavoritesAsync(string id, List<string> teams, List<string> drivers);
    Task CreateUserAsync(User user);
}

public interface IPostService
{
    Task<PostDto> CreatePostAsync(CreatePostDto dto, string authorId, string authorRole, string? repostOfId = null);
    Task<PostDto?> GetPostByIdAsync(string id);
    Task LikePostAsync(string postId, string userId);
    Task RepostPostAsync(string postId, string userId, string userRole);
}


public interface IMediaService { }
public interface INotificationService { }
public interface IFeedService { }

