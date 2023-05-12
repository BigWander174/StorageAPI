using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StorageAPI.Contexts;
using StorageAPI.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StorageContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresStorage"));
});

builder.Services.AddSingleton<PasswordHasher<User>>();

var app = builder.Build();
app.Run();