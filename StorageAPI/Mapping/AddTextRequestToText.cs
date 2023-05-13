namespace StorageAPI.Mapping;

public static class AddTextRequestToText
{
    public static Text ToText(this AddTextRequest request, string userEmail)
    {
        return new Text()
        {
            Deletable = request.Deletable,
            Description = request.Description,
            UserEmail = userEmail
        };
    }
}