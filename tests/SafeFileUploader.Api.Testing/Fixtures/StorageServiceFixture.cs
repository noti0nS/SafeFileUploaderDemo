using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using SafeFileUploader.Api.Testing.Helpers;
using SafeFileUploaderWeb.Api.Configuration;
using SafeFileUploaderWeb.Api.Services;
using Testcontainers.FakeGcsServer;

namespace SafeFileUploader.Api.Testing.Fixtures;

public class StorageServiceFixture : IAsyncLifetime
{
    private readonly FakeGcsServerContainer _fakeGcsServerContainer;

    private StorageClient _storageClient = null!;
    
    public IOptions<GoogleOptionsConfig> GoogleOptions { get; }
    public StorageService StorageService { get; private set; } = null!;

    public StorageServiceFixture()
    {
        _fakeGcsServerContainer = new FakeGcsServerBuilder()
            .WithImage("fsouza/fake-gcs-server:1")
            .WithCommand("-backend", "memory")
            .WithCommand("-scheme", "http")
            .Build();
        GoogleOptions = ConfigurationHelper.GetOptions<GoogleOptionsConfig>(GoogleOptionsConfig.Section);
    }

    public async Task InitializeAsync()
    {
        await _fakeGcsServerContainer.StartAsync();
        GoogleOptions.Value.StorageUrl = _fakeGcsServerContainer.GetConnectionString();
        StorageService = new StorageService(GoogleOptions);
        _storageClient = await new StorageService(GoogleOptions).GetAuthenticatedClient();
        await _storageClient.CreateBucketAsync(
            GoogleOptions.Value.ProjectId, GoogleOptions.Value.Bucket);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;

    public async Task RecreateBucketAsync()
    {
        await _storageClient.DeleteBucketAsync(GoogleOptions.Value.Bucket);
        await _storageClient.CreateBucketAsync(
            GoogleOptions.Value.ProjectId, GoogleOptions.Value.Bucket);
    }
}
