namespace StorageAPI.Model;

public class Text
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Description { get; set; } = default!;
    public bool Deletable { get; set; }
    public string UserEmail { get; set; } = default!;
    public User User { get; set; }
}