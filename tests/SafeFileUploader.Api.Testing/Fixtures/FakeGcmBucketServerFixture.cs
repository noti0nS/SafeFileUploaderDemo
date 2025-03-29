using Google.Cloud.Storage.V1;
using SafeFileUploader.Api.Testing.Helpers;
using SafeFileUploaderWeb.Api.Services;

namespace SafeFileUploader.Api.Testing.Fixtures;

public class FakeGcmBucketServerFixture : IAsyncLifetime
{
    private string _projectId = string.Empty;
    private string _bucketName = string.Empty;
    private StorageClient _storageClient = null!;
    
    public async Task InitializeAsync()
    {
        var configuration = ConfigurationHelper.GetConfiguration();
        _projectId = configuration["Google:ProjectId"] ?? throw new ArgumentException(nameof(_projectId));
        _bucketName = configuration["Google:Bucket"] ?? throw new ArgumentNullException(nameof(_bucketName));
        _storageClient = await new StorageService(configuration).GetAuthenticatedClient();
        await _storageClient.CreateBucketAsync(_projectId, _bucketName);
    }
    
    public async Task DisposeAsync()
    {
        await _storageClient.DeleteBucketAsync(_bucketName);
        _storageClient.Dispose();
    }

    public async Task RecreateBucketAsync()
    {
        await _storageClient.DeleteBucketAsync(_bucketName);
        await _storageClient.CreateBucketAsync(_projectId, _bucketName);
    }
}
