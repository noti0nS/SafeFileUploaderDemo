using System.Net;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using SafeFileUploaderWeb.Api.Abstractions;
using SafeFileUploaderWeb.Api.Configuration;
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
    IOptions<GoogleOptionsConfig> googleOptions) : IFileUploaderHandler
{
    public async Task<ApiResponse<List<UrlPreSignedFileDto>>> GetSignedUrlForFilesAsync(
        UploadFilesRequest request, CancellationToken cancellationToken = default)
    {
        string? errorMessage = request.Validate();
        if (!string.IsNullOrWhiteSpace(errorMessage))
            return ApiResponse<List<UrlPreSignedFileDto>>.Fail(errorMessage, HttpStatusCode.BadRequest);
        
        var signedUrls = new List<UrlPreSignedFileDto>();
        var client = await storageService.GetAuthenticatedClient();
        var signer = client.CreateUrlSigner();
        var preSignedUrlDuration 
            = TimeSpan.FromMinutes(googleOptions.Value.PreSignedUrlDurationInMinutes);
        foreach (var file in request.Files)
        {
            var userFile = new UserFile
            {
                FileName = Path.GetFileNameWithoutExtension(file.Name),
                Extension = Path.GetExtension(file.Name),
                FileSize = file.FileSizeBytes,
            };
            await context.UserFiles.AddAsync(userFile, cancellationToken);
            var url = await signer.SignAsync(
                googleOptions.Value.Bucket,
                userFile.GetFileBucketName(),
                preSignedUrlDuration,
                HttpMethod.Put,
                SigningVersion.V4,
                cancellationToken);
            signedUrls.Add(new(file.Name, url));
        }
        await context.SaveChangesAsync(cancellationToken);
        return ApiResponse<List<UrlPreSignedFileDto>>.Success(signedUrls);
    }
}
