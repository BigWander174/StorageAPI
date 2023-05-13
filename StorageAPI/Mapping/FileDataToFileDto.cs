using StorageAPI.Dtos;

namespace StorageAPI.Mapping;

public static class FileDataToFileDto
{
    public static FileDto ToFileDto(this FileData fileData)
    {
        return new FileDto()
        {
            Id = fileData.Id,
            Deletable = fileData.Deletable,
            Name = fileData.Name
        };
    }
}