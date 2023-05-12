namespace StorageAPI.Services;

public class OsFileStorageService : IFileStorageService
{
    public FileStream? GetFile(FileData fileData)
    {
        return File.Exists(fileData.Path) == false 
            ? null 
            : File.OpenRead(fileData.Path);
    }

    public async Task<string> UploadFile(IFormFile formFile, string userEmail)
    {
        var userDirectoryPath = CalculateUserDirectory(userEmail);
        if (File.Exists(userDirectoryPath) == false)
        {
            Directory.CreateDirectory(userDirectoryPath);
        }

        var fullPath = userDirectoryPath + "/" + formFile.FileName;
        if (File.Exists(fullPath))
        {
            return null;
        }
        
        await using var fileStream = new FileStream(fullPath, FileMode.Create);
        
        await formFile.CopyToAsync(fileStream);
        return fullPath;
    }

    public void DeleteFile(string filePath)
    {
        File.Delete(filePath);
    }

    private static string CalculateUserDirectory(string userEmail)
    {
        return Directory.GetCurrentDirectory() + "/FileStorage/" + userEmail;
    }
}