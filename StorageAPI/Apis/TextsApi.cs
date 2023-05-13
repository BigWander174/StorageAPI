using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace StorageAPI.Apis;

public class TextsApi : IApi
{
    public void Configure(WebApplication app)
    {
        app.MapGet("/texts", GetAllTextsAsync);
        app.MapGet("/texts/{id}", GetTextAsync);
        app.MapPost("/texts", AddTextAsync);
        app.MapDelete("/texts/{id}", DeleteTextAsync);
    }
    
    [Authorize]
    private static async Task<IResult> GetAllTextsAsync(HttpContext context,  
        [FromServices] ITextRepository textRepository)
    {
        var texts = await textRepository.GetAllTextsAsync(GetCurrentUserEmail(context));
        return Results.Json(texts);
    }
    
    [AllowAnonymous]
    private static async Task<IResult> GetTextAsync([FromRoute] string id, HttpContext context, 
        [FromServices] ITextRepository textRepository)
    {
        if (Guid.TryParse(id, out var targetGuid) == false)
        {
            return Results.NotFound();
        }
            
        var targetText = await textRepository.GetTextAsync(targetGuid);
        if (targetText is null)
        {
            return Results.NotFound();
        }

        if (targetText.Deletable)
        {
            await textRepository.RemoveAsync(targetGuid);
        }
        
        return Results.Ok(targetText.Description);
    }
    
    [Authorize]
    private static async Task<IResult> AddTextAsync(HttpContext context, 
        [FromBody] AddTextRequest? request,  
        [FromServices] ITextRepository textRepository, 
        [FromServices] AbstractValidator<AddTextRequest> validator)
    {
        if (request is null)
        {
            return Results.BadRequest("You need to write text info in request body");
        }
        var validationResult = await validator.ValidateAsync(request);
        if (validationResult.IsValid == false)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        
        var id = await textRepository.AddAsync(request, GetCurrentUserEmail(context));
        var downloadLink = context.Request.GetDisplayUrl() + "/" + HttpUtility.UrlEncode(id.ToString());
        return Results.Ok(downloadLink);
    }

    [Authorize]
    private static async Task<IResult> DeleteTextAsync(HttpContext context, [FromRoute] string id,
        [FromServices] ITextRepository textRepository)
    {
        if (Guid.TryParse(id, out var targetGuid) == false)
        {
            return Results.NotFound();
        }
        
        var textToDelete = await textRepository.GetTextAsync(targetGuid);
        if (textToDelete is null)
        {
            return Results.NotFound();
        }
        
        if (textToDelete.UserEmail != GetCurrentUserEmail(context))
        {
            return Results.Unauthorized();
        }
        
        await textRepository.RemoveAsync(targetGuid);
        return Results.Ok();
    }

    private static string? GetCurrentUserEmail(HttpContext context)
        => context.User.FindFirst(ClaimTypes.Email)?.Value;
}