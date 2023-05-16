namespace StorageApi.Tests.Apis;

public class AuthTests : WebApplicationFactory<Program>
{
    private readonly StorageContext _context;
    private readonly User _testUser;
    
    public AuthTests()
    {
        _context = Services.CreateScope().ServiceProvider.GetRequiredService<StorageContext>();

        _testUser = Consts.AuthRequest.ToUser();
        _testUser.Password = new PasswordHasher<User>().HashPassword(_testUser, _testUser.Password);
    }

    [Fact]
    public async Task Login_ReturnsNotFound_WhenUserNotExist()
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("/auth/login", Consts.AuthRequest);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenUserExists()
    {
        await CreateTestUser();

        using var client = CreateClient();
        var response = await client.PostAsJsonAsync("/auth/login", Consts.AuthRequest);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(await response.Content.ReadAsStringAsync());
        await DeleteTestUser();
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenUserNotExists()
    {
        using var client = CreateClient();
        var response = await client.PostAsJsonAsync("/auth/register", Consts.AuthRequest);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(await response.Content.ReadAsStringAsync());

        await DeleteTestUser();
    }

    [Fact]
    public async Task Register_ReturnsConflict_WhenUserExists()
    {
        await CreateTestUser();

        using var client = CreateClient();

        var response = await client.PostAsJsonAsync("/auth/register", Consts.AuthRequest);

        const string errorMessage = "\"User with such email already exists\"";

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(errorMessage, await response.Content.ReadAsStringAsync());

        await DeleteTestUser();
    }

    [Fact]
    public async Task AccessAuthorizedEndpoint_ReturnsUnauthorized_WhenUnauthorized()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/texts");
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Auth_ReturnsBadRequest_WhenBodyIsNull()
    {
        using var client = CreateClient();

        var response = await client.PostAsync("/auth/login", null);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        const string errorMessage = "\"You need to write a login and password in request body\"";
        Assert.Equal(errorMessage, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Auth_ReturnsBadRequest_WhenBodyInvalid()
    {
        using var client = CreateClient();

        var response = await client.PostAsJsonAsync("/auth/login", Consts.AuthRequestWithNoEmail);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotEmpty(await response.Content.ReadAsStringAsync());
    }

    private async Task CreateTestUser()
    {
        await _context.Users.AddAsync(_testUser);
        await _context.SaveChangesAsync();
    }

    private async Task DeleteTestUser()
    {
        _context.Users.Remove(_testUser);
        await _context.SaveChangesAsync();
    }
}