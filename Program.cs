using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StorageAPI.Apis;
using StorageAPI.Contexts;
using StorageAPI.Model;
using StorageAPI.Repositories;
using StorageAPI.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StorageContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresStorage"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<PasswordHasher<User>>();

builder.Services.AddScoped<IUserRepository, DbUserRepository>();
builder.Services.AddScoped<ITextRepository, DbTextRepository>();

builder.Services.AddTransient<IApi, AuthApi>();
builder.Services.AddTransient<IApi, TextsApi>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

using var scope = app.Services.CreateScope();
var apis = scope.ServiceProvider.GetServices<IApi>();
foreach (var api in apis)
{
    api.Configure(app);
}

app.Run();