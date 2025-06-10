using KeepTheApex.DTOs;
using Microsoft.Azure.Cosmos;
using User = KeepTheApex.Models.User;

namespace KeepTheApex.Services;

public class UserService : IUserService
{
    private readonly Container _container;

    public UserService(CosmosClient cosmosClient)
    {
        var databaseId = "KeepTheApexDb";
        var containerId = "Users";
        var partitionKeyPath = "/userId";

        try
        {
            var database = cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            _container = database.Database.CreateContainerIfNotExistsAsync(
                id: containerId,
                partitionKeyPath: partitionKeyPath,
                throughput: 400
            ).Result.Container;

            Console.WriteLine("Connected to or created Cosmos DB container: Users");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to connect or create Cosmos DB container: " + ex.Message);
            throw;
        }
    }
    
    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<User>(id, new PartitionKey(id));
            var user = response.Resource;

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role,
                FavoriteTeams = user.FavoriteTeams,
                FavoriteDrivers = user.FavoriteDrivers,
                RepostedPostIds = user.RepostedPostIds,
                LikedPostIds = user.LikedPostIds
            };
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine($"User not found: {id}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching user: " + ex.Message);
            return null;
        }
    }

    public async Task UpdateFavoritesAsync(string id, List<string> teams, List<string> drivers)
    {
        try
        {
            var response = await _container.ReadItemAsync<User>(id, new PartitionKey(id));
            var user = response.Resource;

            user.FavoriteTeams = teams;
            user.FavoriteDrivers = drivers;

            await _container.ReplaceItemAsync(user, id, new PartitionKey(id));
            Console.WriteLine($"Updated favorites for user: {id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to update user favorites: " + ex.Message);
            throw;
        }
    }
    
    public async Task CreateUserAsync(User user)
    {
        await _container.CreateItemAsync(user, new PartitionKey(user.UserId));
    }

}
