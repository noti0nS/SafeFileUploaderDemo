using SafeFileUploaderWeb.Core.DTOs;
using SafeFileUploaderWeb.Core.Requests;
using SafeFileUploaderWeb.Core.Responses;

namespace SafeFileUploaderWeb.Core.Abstractions;

public interface IFileUploaderHandler
{
    /// <summary>
    /// Save one or more files into the storage and returns a list of presigned urls associated with its files.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ApiResponse<List<UrlPreSignedFileDto>>> GetSignedUrlForFilesAsync(UploadFilesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Save uploaded file references into the storage.
    /// </summary>
    /// <param name="UniqueFileNames"></param>
    /// <returns></returns>
    Task<ApiResponse<bool>> SaveFilesAsync(List<string> UniqueFileNames);
}
