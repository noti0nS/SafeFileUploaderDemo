using Google.Cloud.Storage.V1;
using SafeFileUploader.Api.Testing.Helpers;
using SafeFileUploaderWeb.Api.Services;
using Testcontainers.FakeGcsServer;

namespace SafeFileUploader.Api.Testing.Fixtures;

public class FakeGcmBucketServerFixture : IAsyncLifetime
{
    private readonly FakeGcsServerContainer _fakeGcsServerContainer;
    
    private string _projectId = string.Empty;
    private string _bucketName = string.Empty;
    private StorageClient _storageClient = null!;

    public FakeGcmBucketServerFixture()
    {
        _fakeGcsServerContainer = new FakeGcsServerBuilder()
            .WithImage("fsouza/fake-gcs-server:1")
            .WithCommand("-backend", "memory")
            .WithCommand("-scheme", "http")
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await _fakeGcsServerContainer.StartAsync();

        _projectId = 
            ConfigurationHelper.Configuration["Google:ProjectId"] ?? throw new ArgumentException(nameof(_projectId));
        _bucketName = 
            ConfigurationHelper.Configuration["Google:Bucket"] ?? throw new ArgumentNullException(nameof(_bucketName));
        ConfigurationHelper.SetStorageUrl(_fakeGcsServerContainer.GetConnectionString());

        _storageClient = await new StorageService(ConfigurationHelper.Configuration).GetAuthenticatedClient();
        await _storageClient.CreateBucketAsync(_projectId, _bucketName);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;

    public async Task RecreateBucketAsync()
    {
        await _storageClient.DeleteBucketAsync(_bucketName);
        await _storageClient.CreateBucketAsync(_projectId, _bucketName);
    }
}
