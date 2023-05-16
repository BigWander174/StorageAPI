namespace StorageApi.Tests.Apis;

public class TextsTests : WebApplicationFactory<Program>
{
    private readonly StorageContext _context;

    public TextsTests()
    {
        _context = Services.CreateScope().ServiceProvider.GetRequiredService<StorageContext>();
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
        await AddTextToDb(Consts.DeletableText);

        using var client = CreateClient();
        var response = await client.GetAsync($"texts/{Consts.DeletableText.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(_context.Texts.FirstOrDefault(t => t.Id == Consts.DeletableText.Id));
    }
    
    [Fact]
    public async Task GetNotDeletableText_ReturnsOk_WhenTextExist()
    {
        var text = Consts.NotDeletableText;
        await AddTextToDb(text);

        using var client = CreateClient();
        var response = await client.GetAsync($"texts/{text.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(_context.Texts.FirstOrDefault(t => t.Id == text.Id));
        await RemoveTextFromDb(text);
    }
    
    [Fact]
    public async Task UploadText_ReturnOk_WhenAuthorized()
    {
        using var client = CreateClientWithAuthHeader();

        var response = await client.PostAsJsonAsync("/texts", Consts.AddTextRequest);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(await response.Content.ReadAsStringAsync());

        
        _context.Texts.Remove(_context.Texts.FirstOrDefault(text =>
            text.Description == Consts.AddTextRequest.Description));
        await _context.SaveChangesAsync();
    }
    
    [Fact]
    public async Task DeleteText_ReturnsOk_WhenTextExist()
    {
        AddTextToDb(Consts.DeletableText);

        using var client = CreateClientWithAuthHeader();

        var response = await client.DeleteAsync($"/texts/{Consts.DeletableText.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(_context.Texts.FirstOrDefault(t => t.Id == Consts.DeletableText.Id));
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
    
    private async Task AddTextToDb(Text text)
    {
        await _context.Texts.AddAsync(text);
        await _context.SaveChangesAsync();
    }

    private async Task RemoveTextFromDb(Text text)
    {
        _context.Texts.Remove(text);
        await _context.SaveChangesAsync();
    }
}