using System.Net;
using Google.Cloud.Storage.V1;
using SafeFileUploaderWeb.Api.Abstractions;
using SafeFileUploaderWeb.Api.Data;
using SafeFileUploaderWeb.Core.Abstractions;
using SafeFileUploaderWeb.Core.DTOs;
using SafeFileUploaderWeb.Core.Entities;
using SafeFileUploaderWeb.Core.Requests;
using SafeFileUploaderWeb.Core.Responses;

namespace SafeFileUploaderWeb.Api.Handlers;

public class FileUploaderHandler(
    DatabaseContext context,
    IStorageService storageService,
    IConfiguration configuration) : IFileUploaderHandler
{
    public async Task<ApiResponse<List<UrlPreSignedFileDto>>> GetSignedUrlForFilesAsync(
        UploadFilesRequest request, CancellationToken cancellationToken = default)
    {
        string? errorMessage = request.Validate();
        if (!string.IsNullOrWhiteSpace(errorMessage))
            return ApiResponse<List<UrlPreSignedFileDto>>.Fail(errorMessage, HttpStatusCode.BadRequest);
        
        var client = await storageService.GetAuthenticatedClient();
        var signer = client.CreateUrlSigner();
        var userFiles = await SaveFilesIntoDb(request, cancellationToken);
        
        var bucket = configuration["Google:Bucket"];
        var preSignedUrlDuration 
            = TimeSpan.FromMinutes(configuration.GetValue<int>("Google:PreSignedUrlDurationInMinutes"));
        var signedUrls = new List<UrlPreSignedFileDto>();
        foreach (var userFile in userFiles)
        {
            var url = await signer.SignAsync(
                bucket, 
                userFile.GetFileBucketName(), 
                preSignedUrlDuration, 
                HttpMethod.Put, 
                SigningVersion.V4, 
                cancellationToken);
            signedUrls.Add(new UrlPreSignedFileDto(userFile.GetFileNameWithExtension(), url));
        }
        return ApiResponse<List<UrlPreSignedFileDto>>.Success(signedUrls);
    }

    private async Task<List<UserFile>> SaveFilesIntoDb(
        UploadFilesRequest request, CancellationToken cancellationToken = default)
    {
        List<UserFile> userFiles = [];
        foreach (var file in request.Files)
        {
            var userFile = new UserFile
            {
                FileName = Path.GetFileNameWithoutExtension(file.Name),
                Extension = Path.GetExtension(file.Name),
                FileSize = file.FileSizeBytes,
            };
            await context.AddAsync(userFile, cancellationToken);
            userFiles.Add(userFile);
        }
        await context.SaveChangesAsync(cancellationToken);
        return userFiles;
    }
}
