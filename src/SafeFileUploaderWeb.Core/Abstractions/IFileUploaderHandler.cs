using SafeFileUploaderWeb.Core.DTOs;
using SafeFileUploaderWeb.Core.Requests;
using SafeFileUploaderWeb.Core.Responses;

namespace SafeFileUploaderWeb.Core.Abstractions;

public interface IFileUploaderHandler
{
    /// <summary>
    /// Save one or more files into the storage and returns a list of pre-signed urls associated with its files.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiResponse<List<UrlPreSignedFileDto>>> GetSignedUrlForFilesAsync(
        UploadFilesRequest request, CancellationToken cancellationToken = default);
}
