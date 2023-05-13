namespace StorageAPI.Dtos;

public class TextDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = default!;
    public bool Deletable { get; set; }
}