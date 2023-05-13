using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StorageAPI.Apis;

public class FilesApi : IApi
{
    public void Configure(WebApplication app)
    {
        app.MapGet("/files", GetFiles);
        app.MapGet("/files/{id}", GetFile);
        app.MapPost("/files", AddFile);
        app.MapDelete("/files/{id}", DeleteFile);
    }
    
    [Authorize]
    private static async Task<IResult> GetFiles(HttpContext context,  
        [FromServices] IFileDataRepository fileDataRepository)
    {
        var files = await fileDataRepository.GetAllFilesAsync(GetCurrentUserEmail(context));
        return Results.Json(files);
    }

    [AllowAnonymous]
    private static async Task<IResult> GetFile([FromRoute] string id, 
         HttpContext context, 
        [FromServices] IFileStorageService fileStorageService, 
        [FromServices] IFileDataRepository fileDataRepository)
    {
        if (Guid.TryParse(id, out var targetGuid) == false)
        {
            return Results.NotFound();
        }
        
        var fileData = await fileDataRepository.GetFile(targetGuid);
        if (fileData is null)
        {
            return Results.NotFound();
        }
        
        var file = fileStorageService.GetFile(fileData);

        if (file is null)
        {
            return Results.NotFound();
        }

        if (fileData.Deletable)
        {
            await fileDataRepository.DeleteFileAsync(fileData);
            fileStorageService.DeleteFile(fileData.Path);
        }
        
        return Results.File(file, "application/octet-stream", fileData.Name);
    }

    [Authorize]
    private static async Task<IResult> AddFile(HttpContext context, 
        [FromServices] IFileStorageService fileStorageService,
        [FromServices] IFileDataRepository fileDataRepository,
        [FromForm] IFormFile? file,
        [FromQuery] bool? deletable)
    {
        if (file is null)
        {
            return Results.BadRequest("Form file cannot be null");
        }

        var filePath = await fileStorageService.UploadFile(file, GetCurrentUserEmail(context));
        if (filePath is null)
        {
            return Results.Conflict("File with such name already exists");
        }
        
        var newFile = new FileData()
        {
            Path = filePath,
            Deletable = deletable ?? false,
            Name = file.FileName,
            UserEmail = GetCurrentUserEmail(context)
        };
        
        await fileDataRepository.AddFileDataAsync(newFile);
        var link = $"{context.Request.Scheme}:/{context.Request.Host}{context.Request.Path}/{newFile.Id}";
        return Results.Ok(link);
    }

    [Authorize]
    private static async Task<IResult> DeleteFile(HttpContext context, 
        [FromRoute] string id,
        [FromServices] IFileStorageService fileService, 
        [FromServices] IFileDataRepository fileDataRepository)
    {
        if (Guid.TryParse(id, out var targetGuid) == false)
        {
            return Results.NotFound();
        }
        
        var fileToDelete = await fileDataRepository.GetFile(targetGuid);
        if (fileToDelete is null)
        {
            return Results.NotFound();
        }
        
        if (fileToDelete.UserEmail != GetCurrentUserEmail(context))
        {
            return Results.Unauthorized();
        }

        await fileDataRepository.DeleteFileAsync(fileToDelete);
        fileService.DeleteFile(fileToDelete.Path);
        
        return Results.Ok();
    }

    private static string? GetCurrentUserEmail(HttpContext context)
        => context.User.FindFirst(ClaimTypes.Email)?.Value;
}