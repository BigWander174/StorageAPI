using StorageAPI.Requests;

namespace StorageAPI.Tests;

public static class Consts
{
    public static AuthRequest AuthRequest = new AuthRequest()
    {
        Login = "test@mail.ru",
        Password = "1234567"
    };

    public static AuthRequest AuthRequestWithNoEmail = new AuthRequest()
    {
        Password = "1234567"
    };
}