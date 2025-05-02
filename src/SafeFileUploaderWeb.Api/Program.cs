using Microsoft.EntityFrameworkCore;
using SafeFileUploaderWeb.Api.Abstractions;
using SafeFileUploaderWeb.Api.Configuration;
using SafeFileUploaderWeb.Api.Data;
using SafeFileUploaderWeb.Api.Extensions;
using SafeFileUploaderWeb.Api.Handlers;
using SafeFileUploaderWeb.Api.Services;
using SafeFileUploaderWeb.Core.Abstractions;
using SafeFileUploaderWeb.Core.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddOptions<GoogleOptionsConfig>()
    .Bind(builder.Configuration.GetSection("Google"));

builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddScoped<IFileUploaderHandler, FileUploaderHandler>();

var app = builder.Build();

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.MapGet("/", () => new { Message = "OK" });

app.MapPost("/api/upload", async (UploadFilesRequest request, IFileUploaderHandler handler) =>
{
    var response = await handler.GetSignedUrlForFilesAsync(request);
    return response.ToHttpResult();
});

app.Run();
