namespace StorageAPI.Services;

public interface IFileStorageService
{
    FileStream? GetFile(FileData fileData);
    Task<string> UploadFile(IFormFile formFile, string userEmail);
    void DeleteFile(string filePath);
}