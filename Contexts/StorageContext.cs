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
    public DbSet<Text> Texts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<User>().HasKey(user => user.Email);
        modelBuilder
            .Entity<User>()
            .HasMany(user => user.Texts)
            .WithOne(text => text.User)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Text>().ToTable("texts");

    }
}