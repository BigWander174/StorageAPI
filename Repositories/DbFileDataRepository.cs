using Microsoft.EntityFrameworkCore;
using StorageAPI.Contexts;
using StorageAPI.Model;
using StorageAPI.Repositories.Interfaces;

namespace StorageAPI.Repositories;

public class DbFileDataRepository : IFileDataRepository
{
    private readonly StorageContext _context;

    public DbFileDataRepository(StorageContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FileData?>> GetAllFilesAsync(string userEmail)
    {
        return await _context.Files.Where(file => file.UserEmail == userEmail).ToListAsync();
    }

    public async Task<FileData?> GetFile(Guid guid)
    {
        return await _context.Files.FindAsync(new object[] { guid });
    }

    public async Task DeleteFileAsync(FileData fileData)
    {
        _context.Files.Remove(fileData);
        await _context.SaveChangesAsync();
    }

    public async Task AddFileDataAsync(FileData fileData)
    {
        await _context.Files.AddAsync(fileData);
        await _context.SaveChangesAsync();
    }
}