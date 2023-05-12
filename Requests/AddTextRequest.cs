namespace StorageAPI.Requests;

public class AddTextRequest
{
    public string Description { get; set; } = default!;
    public bool Deletable { get; set; }
}