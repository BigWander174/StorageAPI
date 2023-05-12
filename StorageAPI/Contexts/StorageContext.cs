namespace StorageAPI.Contexts;

public class StorageContext : DbContext
{
    public StorageContext(DbContextOptions<StorageContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Text> Texts { get; set; }
    public DbSet<FileData> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<User>().HasKey(user => user.Email);

        modelBuilder.Entity<Text>().ToTable("texts");

        modelBuilder.Entity<FileData>().ToTable("files_data");
    }
}