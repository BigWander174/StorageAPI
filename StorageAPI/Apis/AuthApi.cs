using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using StorageAPI.Mapping;

namespace StorageAPI.Apis;

public class AuthApi : IApi
{
    public void Configure(WebApplication app)
    {
        app.MapPost("/auth/login", Login);
        app.MapPost("/auth/register", Register);
    }
    
    private static async Task<IResult> Login([FromBody] AuthRequest? request, [FromServices] IUserRepository userRepository,
        [FromServices] IConfiguration configuration, [FromServices] AbstractValidator<AuthRequest?> validator)
    {
        if (request is null)
        {
            return Results.BadRequest("You need to write a login and password in request body");
        }
        
        var validationResult = await validator.ValidateAsync(request);
        if (validationResult.IsValid == false)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        
        var userFromDb = await userRepository.GetUserByAsync(request);
        return userFromDb is null
            ? Results.NotFound()
            : CreateToken(userFromDb, configuration);
    }
    
    private static async Task<IResult> Register([FromBody] AuthRequest request, 
        [FromServices] IUserRepository userRepository,
        [FromServices] PasswordHasher<User> passwordHasher, 
        [FromServices] IConfiguration configuration, 
        [FromServices] AbstractValidator<AuthRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (validationResult.IsValid == false)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        
        var userFromDb = await userRepository.GetUserByAsync(request.Login);
        if (userFromDb is not null)
        {
            return Results.Conflict("User with such email already exists");
        }

        var user = request.ToUser();
        user.Password = passwordHasher.HashPassword(user, user.Password);
        
        await userRepository.AddAsync(user);

        return CreateToken(user, configuration);
    }
    
    private static IResult CreateToken(User user, IConfiguration configuration)
    {
        var claims = new []
        {
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);
            
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return Results.Ok(jwt);
    }
}