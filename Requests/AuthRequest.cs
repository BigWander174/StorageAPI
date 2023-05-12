namespace StorageAPI.Requests;

public class AuthRequest
{
    public string Login { get; set; } = default!;
    public string Password { get; set; } = default!;
}