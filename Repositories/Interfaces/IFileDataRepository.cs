using StorageAPI.Model;

namespace StorageAPI.Repositories.Interfaces;

public interface IFileDataRepository
{
    Task<IEnumerable<FileData?>> GetAllFilesAsync(string userEmail);
    Task AddFileDataAsync(FileData fileData);
    Task<FileData?> GetFile(Guid guid);
    Task DeleteFileAsync(FileData fileData);
}