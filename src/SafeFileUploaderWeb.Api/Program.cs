using SafeFileUploaderWeb.Api.Abstractions;
using SafeFileUploaderWeb.Api.Handlers;
using SafeFileUploaderWeb.Api.Services;
using SafeFileUploaderWeb.Core.Abstractions;
using SafeFileUploaderWeb.Core.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddScoped<IFileUploaderHandler, FileUploaderHandler>();

var app = builder.Build();

app.MapGet("/", () => new { Message = "OK" });

app.MapPost("/api/upload", async (UploadFilesRequest request, IFileUploaderHandler handler) =>
{
    var response = await handler.GetSignedUrlForFilesAsync(request);
    return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
});

app.Run();
