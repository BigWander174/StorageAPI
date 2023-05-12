namespace StorageAPI.Model;

public class FileData
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Path { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool Deletable { get; set; }
    public string UserEmail { get; set; } = default!;
}