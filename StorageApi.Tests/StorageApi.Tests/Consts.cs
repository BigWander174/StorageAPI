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

    public static AddTextRequest AddDeletableTextRequest = new AddTextRequest()
    {
        Deletable = true,
        Description = "test"
    };

    public static AddTextRequest AddNotDeletableTextRequest = new AddTextRequest()
    {
        Deletable = false,
        Description = "test2"
    };

    public static Text Text = new Text()
    {
        Deletable = true,
        Description = "test",
        UserEmail = AuthRequest.Login
    };
}