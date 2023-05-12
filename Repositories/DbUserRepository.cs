namespace StorageAPI.Repositories;

public class DbUserRepository : IUserRepository
{
    private readonly StorageContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public DbUserRepository(StorageContext context, PasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> GetUserByAsync(AuthRequest request)
    {
        var userWithSameLogin = await GetUserByAsync(request.Login);
        if (userWithSameLogin is null)
        {
            return null;
        }
        
        var isRightPassword = _passwordHasher.VerifyHashedPassword(userWithSameLogin,
                                  userWithSameLogin.Password, request.Password)
                              == PasswordVerificationResult.Success;

        return isRightPassword
            ? userWithSameLogin
            : null;
    }

    public async Task<User?> GetUserByAsync(string login)
    {
        return await _context.Users.FindAsync(new object[]{ login });
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}