using System.Net;
using SafeFileUploaderWeb.Api.Abstractions;
using SafeFileUploaderWeb.Core.Abstractions;
using SafeFileUploaderWeb.Core.DTOs;
using SafeFileUploaderWeb.Core.Requests;
using SafeFileUploaderWeb.Core.Responses;

namespace SafeFileUploaderWeb.Api.Handlers;

public class FileUploaderHandler(IStorageService storageService, IConfiguration configuration) : IFileUploaderHandler
{
    public async Task<ApiResponse<List<UrlPreSignedFileDto>>> GetSignedUrlForFilesAsync(
        UploadFilesRequest request, CancellationToken cancellationToken = default)
    {
        if (HasRepeatedFileNames(request.FileNames))
        {
            return ApiResponse<List<UrlPreSignedFileDto>>.Fail("Files with the same names are not allowed.", HttpStatusCode.BadRequest);
        }
        var signedUrls = new List<UrlPreSignedFileDto>();
        var client = await storageService.GetAuthenticatedClient();
        var signer = client.CreateUrlSigner();
        foreach (var fileName in request.FileNames)
        {
            var uniqueFileName 
                = $"{Guid.NewGuid().ToString().Replace("/", "")}{Path.GetExtension(fileName)}";
            var url = await signer.SignAsync(
                configuration["Google:Bucket"], 
                uniqueFileName, 
                TimeSpan.FromMinutes(configuration.GetValue<int>("Google:PreSignedUrlDurationInMinutes")), 
                HttpMethod.Put, 
                cancellationToken: cancellationToken);
            signedUrls.Add(new UrlPreSignedFileDto(fileName, uniqueFileName, url));
        }
        return ApiResponse<List<UrlPreSignedFileDto>>.Success(signedUrls);
    }

    private static bool HasRepeatedFileNames(List<string> fileNames)
    {
        HashSet<string> addedFiles = [];
        foreach (var fileName in fileNames)
            if (!addedFiles.Add(fileName)) return true;
        return false;
    }

    public Task<ApiResponse<bool>> SaveFilesAsync(List<string> UniqueFileNames)
    {
        throw new NotImplementedException();
    }
}
