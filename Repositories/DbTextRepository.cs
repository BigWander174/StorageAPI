namespace StorageAPI.Repositories;

public class DbTextRepository : ITextRepository
{
    private readonly StorageContext _context;

    public DbTextRepository(StorageContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Text>> GetAllTextsAsync(string? userEmail)
    {
        return await _context.Texts.Where(text => text.UserEmail == userEmail).ToListAsync();
    }

    public async Task<Guid> AddAsync(AddTextRequest addTextRequest, string? userEmail)
    {
        var newText = new Text()
        {
            Description = addTextRequest.Description,
            Deletable = addTextRequest.Deletable,
            UserEmail = userEmail
        };
        await _context.Texts.AddAsync(newText);
        await _context.SaveChangesAsync();
        return newText.Id;
    }

    public async Task<Text?> GetTextAsync(Guid guid)
    {
        return await _context.Texts.FirstOrDefaultAsync(text => text.Id == guid);
    }

    public async Task RemoveAsync(Guid guid)
    {
        var textToDelete = await _context.Texts.FindAsync(new object[]{ guid });
        if (textToDelete != null) 
            _context.Texts.Remove(textToDelete);
        await _context.SaveChangesAsync();
    }
}