using Microsoft.EntityFrameworkCore;
using StorageAPI.Model;

namespace StorageAPI.Contexts;

public class StorageContext : DbContext
{
    public StorageContext(DbContextOptions<StorageContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(user => user.Email);
        modelBuilder.Entity<User>().ToTable("users");

    }
}