namespace StorageAPI.Mapping;

public static class AuthRequestToUser
{
    public static User ToUser(this AuthRequest request)
    {
        return new User()
        {
            Email = request.Login,
            Password = request.Password
        };
    }
}