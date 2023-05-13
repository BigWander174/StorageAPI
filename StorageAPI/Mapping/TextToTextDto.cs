using StorageAPI.Dtos;

namespace StorageAPI.Mapping;

public static class TextToTextDto
{
    public static TextDto ToTextDto(this Text text)
    {
        return new TextDto()
        {
            Id = text.Id,
            Description = text.Description,
            Deletable = text.Deletable
        };
    }
}