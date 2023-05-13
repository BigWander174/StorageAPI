namespace StorageAPI.Dtos;

public class FileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool Deletable { get; set; }
}