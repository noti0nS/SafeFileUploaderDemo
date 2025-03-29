using Google.Cloud.Storage.V1;

namespace SafeFileUploaderWeb.Api.Abstractions;

public interface IStorageService
{
    Task<StorageClient> GetAuthenticatedClient();
}
