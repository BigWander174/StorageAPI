namespace StorageAPI.Model;

public class User
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public List<Text> Texts { get; set; }
}