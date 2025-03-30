using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using SafeFileUploaderWeb.Api.Abstractions;

namespace SafeFileUploaderWeb.Api.Services;

public class StorageService(IConfiguration configuration) : IStorageService
{
    private StorageClient? _storageClient;

    public async Task<StorageClient> GetAuthenticatedClient()
    {
        if (_storageClient is null)
        {
            var initializer = new ServiceAccountCredential
                .Initializer(configuration["Google:ClientEmail"])
                .FromPrivateKey(configuration["Google:PrivateKey"]); 
            var clientBuilder = new StorageClientBuilder
            {
                UnauthenticatedAccess = false,
                BaseUri = configuration["Google:StorageUrl"],
                Credential = GoogleCredential.FromServiceAccountCredential(new ServiceAccountCredential(initializer))
            };
            _storageClient = await clientBuilder.BuildAsync();
        }
        return _storageClient;
    }
}
