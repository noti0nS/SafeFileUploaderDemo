using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SafeFileUploader.Api.Testing.Fixtures;
using SafeFileUploader.Api.Testing.Helpers;
using SafeFileUploaderWeb.Api.Handlers;
using SafeFileUploaderWeb.Api.Services;
using SafeFileUploaderWeb.Core.Requests;

namespace SafeFileUploader.Api.Testing;

public class FileUploaderHandlerTest : IClassFixture<FakeGcmBucketServerFixture>, IAsyncLifetime
{
    private readonly IConfiguration _configuration;
    private readonly FakeGcmBucketServerFixture _storageFixture;

    public FileUploaderHandlerTest(FakeGcmBucketServerFixture fixture)
    {
        _storageFixture = fixture;
        _configuration = ConfigurationHelper.GetConfiguration();
    }
    
    Task IAsyncLifetime.InitializeAsync() => Task.CompletedTask;
    
    async Task IAsyncLifetime.DisposeAsync() 
        => await _storageFixture.RecreateBucketAsync();
    
    [Fact]
    public async Task GetSignedUrlForFilesAsync_InputValidFiles_OutputSignedUrls()
    {
        var handler = new FileUploaderHandler(new StorageService(_configuration), _configuration);

        var request = new UploadFilesRequest(["my_file1.txt", "my_file2.png", "my_file3.pdf"]);
        var result = await handler.GetSignedUrlForFilesAsync(request);

        result.IsSuccess.Should().Be(true);
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Data.Select(r => r.OriginalFileName).Should().BeEquivalentTo(request.FileNames);
        result.Data.All(r => Path.GetExtension(r.UniqueFileName) == Path.GetExtension(r.OriginalFileName))
            .Should().BeTrue("all unique filenames should preserve the original extension");
    }
    
    [Fact]
    public async Task GetSignedUrlForFilesAsync_RepeatedFileName_ReturnsBadRequest()
    {
        var handler = new FileUploaderHandler(new StorageService(_configuration), _configuration);

        var request = new UploadFilesRequest(["my_file1.txt", "my_file1.txt", "my_file3.pdf"]);
        var result = await handler.GetSignedUrlForFilesAsync(request);

        result.IsSuccess.Should().Be(false);
        result.Code.Should().Be(HttpStatusCode.BadRequest);
        result.Data.Should().BeNull(); 
    }
}
