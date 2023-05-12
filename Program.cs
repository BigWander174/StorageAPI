using Microsoft.EntityFrameworkCore;
using StorageAPI.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StorageContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresStorage"));
});

var app = builder.Build();
app.Run();