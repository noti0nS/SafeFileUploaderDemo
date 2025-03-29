using SafeFileUploaderWeb.Core.Abstractions;
using SafeFileUploaderWeb.Core.DTOs;
using SafeFileUploaderWeb.Core.Requests;
using SafeFileUploaderWeb.Core.Responses;

namespace SafeFileUploaderWeb.Web.Handlers;

public class FileUploaderHandler : IFileUploaderHandler
{
    public Task<ApiResponse<List<UrlPreSignedFileDto>>> GetSignedUrlForFilesAsync(UploadFilesRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<List<UrlPreSignedFileDto>>> GetSignedUrlForFilesAsync(UploadFilesRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<bool>> SaveFilesAsync(List<string> UniqueFileNames)
    {
        throw new NotImplementedException();
    }
}
