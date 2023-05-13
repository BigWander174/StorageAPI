namespace StorageAPI.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByAsync(AuthRequest? request);
    Task<User?> GetUserByAsync(string login);
    Task AddAsync(User user);
}