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

    public static AddTextRequest AddTextRequest = new AddTextRequest()
    {
        Deletable = true,
        Description = "test"
    };

    public static Text DeletableText = new Text()
    {
        Deletable = true,
        Description = "test",
        UserEmail = AuthRequest.Login
    };
    
    public static Text NotDeletableText = new Text()
    {
        Deletable = false,
        Description = "test2",
        UserEmail = AuthRequest.Login
    };
}