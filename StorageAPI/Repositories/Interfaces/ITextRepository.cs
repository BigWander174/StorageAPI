namespace StorageAPI.Repositories.Interfaces;

public interface ITextRepository
{
    Task<IEnumerable<Text>> GetAllTextsAsync(string? userEmail);
    Task<Guid> AddAsync(AddTextRequest request, string? userEmail);
    Task<Text?> GetTextAsync(Guid guid);
    Task RemoveAsync(Guid guid);
}