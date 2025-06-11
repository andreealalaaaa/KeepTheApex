using KeepTheApex.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using User = KeepTheApex.Models.User;

namespace KeepTheApex.Services;

public class UserService : IUserService
{
    private readonly Container _container;
    private readonly PasswordHasher<User> _hasher;
    
    public UserService(CosmosClient cosmosClient)
    {
        _hasher  = new PasswordHasher<User>();
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

    public async Task<List<User>> GetAllUsersAsync()
    {
        var query = new QueryDefinition("SELECT * FROM c");
        var iterator = _container.GetItemQueryIterator<User>(query);
        var users = new List<User>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            users.AddRange(response);
        }

        return users;
    }
    
    public async Task<UserDto> RegisterUserAsync(RegisterUserDto dto)
    {
        // 1) build domain model
        var id   = Guid.NewGuid().ToString();
        var user = new User
        {
            Id                 = id,
            UserId             = id,
            Username           = dto.FullName,
            Email              = dto.Email,
            Role               = dto.Role,
            FavoriteTeams      = new List<string>(),
            FavoriteDrivers    = new List<string>(),
            RepostedPostIds    = new List<string>(),
            LikedPostIds       = new List<string>()
        };

        // 2) hash & store
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);
        await _container.CreateItemAsync(user, new PartitionKey(user.UserId));

        // 3) return a DTO (without the hash!)
        return new UserDto
        {
            UserId           = user.UserId,
            Username         = user.Username,
            Role             = user.Role,
            FavoriteTeams    = user.FavoriteTeams,
            FavoriteDrivers  = user.FavoriteDrivers,
            RepostedPostIds  = user.RepostedPostIds,
            LikedPostIds     = user.LikedPostIds
        };
    }
    
    public async Task<UserDto?> LoginAsync(LoginDto dto)
    {
        // find by email
        var queryText = "SELECT * FROM c WHERE c.email = @email";
        var queryDef  = new QueryDefinition(queryText)
            .WithParameter("@email", dto.Email);
        var it        = _container.GetItemQueryIterator<User>(queryDef);
        var results   = await it.ReadNextAsync();
        var user      = results.FirstOrDefault();
        if (user == null)
            return null;

        // verify password
        var check = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (check == PasswordVerificationResult.Failed)
            return null;

        // return the same DTO shape as GetUser
        return new UserDto
        {
            UserId           = user.UserId,
            Username         = user.Username,
            Role             = user.Role,
            FavoriteTeams    = user.FavoriteTeams,
            FavoriteDrivers  = user.FavoriteDrivers,
            RepostedPostIds  = user.RepostedPostIds,
            LikedPostIds     = user.LikedPostIds
        };
    }
}
