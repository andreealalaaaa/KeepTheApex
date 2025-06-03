using KeepTheApex.DTOs;

namespace KeepTheApex.Services;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string id);
    Task UpdateFavoritesAsync(string id, List<string> teams, List<string> drivers);
}

public interface IPostService { }
public interface IMediaService { }
public interface INotificationService { }
public interface IFeedService { }

