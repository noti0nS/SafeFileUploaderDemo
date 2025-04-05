using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using SafeFileUploaderWeb.Api.Abstractions;
using SafeFileUploaderWeb.Api.Configuration;

namespace SafeFileUploaderWeb.Api.Services;

public class StorageService(IOptions<GoogleOptionsConfig> googleOptions) : IStorageService
{
    private StorageClient? _storageClient;

    public async Task<StorageClient> GetAuthenticatedClient()
    {
        if (_storageClient is null)
        {
            var initializer = new ServiceAccountCredential
                .Initializer(googleOptions.Value.ClientEmail)
                .FromPrivateKey(googleOptions.Value.PrivateKey); 
            var clientBuilder = new StorageClientBuilder
            {
                UnauthenticatedAccess = false,
                BaseUri = googleOptions.Value.StorageUrl,
                Credential = GoogleCredential.FromServiceAccountCredential(new ServiceAccountCredential(initializer))
            };
            _storageClient = await clientBuilder.BuildAsync();
        }
        return _storageClient;
    }
}
