namespace StorageApi.Tests.Apis;

public class TextsTests : WebApplicationFactory<Program>
{
    private readonly StorageContext _context;
    private readonly User _testUser;
    private readonly Text _testText;

    public TextsTests()
    {
        _context = Services.CreateScope().ServiceProvider.GetRequiredService<StorageContext>();
        _testUser = Consts.AuthRequest.ToUser();
        _testUser.Password = new PasswordHasher<User>().HashPassword(_testUser, _testUser.Password);
    }

    [Fact]
    public async Task GetTexts_ReturnsOk_WhenAuthorized()
    {
        using var client = CreateClientWithAuthHeader();

        var response = await client.GetAsync("/texts");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(await response.Content.ReadAsStringAsync());
    }
    
    [Fact]
    public async Task GetText_ReturnsNotFound_WhenTextNotExist()
    {
        using var client = CreateClient();

        const int id = -1;
        var response = await client.GetAsync($"/texts/{id}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDeletableText_ReturnsOk_WhenTextExist()
    {
        var userEmail = Consts.AuthRequest.Login;
        var text = await AddTextToDb(Consts.AddDeletableTextRequest.ToText(userEmail));

        using var client = CreateClient();
        var response = await client.GetAsync($"texts/{text.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(_context.Texts.FirstOrDefault(t => t.Id == text.Id));
    }
    
    [Fact]
    public async Task GetNotDeletableText_ReturnsOk_WhenTextExist()
    {
        var userEmail = Consts.AuthRequest.Login;
        var text = await AddTextToDb(Consts.AddNotDeletableTextRequest.ToText(userEmail));

        using var client = CreateClient();
        var response = await client.GetAsync($"texts/{text.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(_context.Texts.FirstOrDefault(t => t.Id == text.Id));
        RemoveTextFromDb(_context.Texts.First(t => t.Id == text.Id));
    }
    
    [Fact]
    public async Task UploadText_ReturnOk_WhenAuthorized()
    {
        using var client = CreateClientWithAuthHeader();

        var response = await client.PostAsJsonAsync("/texts", Consts.AddDeletableTextRequest);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(await response.Content.ReadAsStringAsync());
    }
    
    [Fact]
    public async Task DeleteText_ReturnsOk_WhenTextExist()
    {
        var text = await AddTextToDb(Consts.Text);

        using var client = CreateClientWithAuthHeader();

        var response = await client.DeleteAsync($"/texts/{text.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(_context.Texts.FirstOrDefault(t => t.Id == text.Id));
    }

    [Fact]
    public async Task DeleteText_ReturnsNotFound_WhenTextNotExist()
    {
        using var client = CreateClientWithAuthHeader();

        const int id = -1;
        var response = await client.DeleteAsync($"/texts/{id}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private HttpClient CreateClientWithAuthHeader()
    {
        var client = CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken());

        return client;
    }

    private string CreateToken()
    {
        var configuration = Services.GetRequiredService<IConfiguration>();
        var claims = new []
        {
            new Claim(ClaimTypes.Email, Consts.AuthRequest.Login)
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
        return jwt;
    }
    
    private async Task<Text> AddTextToDb(Text text)
    {
        await _context.Texts.AddAsync(text);
        await _context.SaveChangesAsync();

        return text;
    }

    private void RemoveTextFromDb(Text text)
    {
        _context.Texts.Remove(text);
        _context.SaveChanges();
    }
}