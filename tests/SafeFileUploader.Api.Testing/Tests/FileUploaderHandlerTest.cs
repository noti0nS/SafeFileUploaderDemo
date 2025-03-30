using System.Net;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SafeFileUploader.Api.Testing.Fixtures;
using SafeFileUploader.Api.Testing.Helpers;
using SafeFileUploaderWeb.Api.Handlers;
using SafeFileUploaderWeb.Api.Services;
using SafeFileUploaderWeb.Core.Requests;

namespace SafeFileUploader.Api.Testing.Tests;

public class FileUploaderHandlerTest(MsSqlDbContextFixture contextFixture, FakeGcmBucketServerFixture storageFixture)
    : IClassFixture<FakeGcmBucketServerFixture>, IClassFixture<MsSqlDbContextFixture>, IAsyncLifetime
{
    Task IAsyncLifetime.InitializeAsync() => Task.CompletedTask;

    async Task IAsyncLifetime.DisposeAsync()
    {
        await contextFixture.CleanUpDataAsync();
        await storageFixture.RecreateBucketAsync();
    }
    
    [Fact]
    public async Task GetSignedUrlForFilesAsync_InputValidFiles_OutputSignedUrls()
    {
        var handler = new FileUploaderHandler(
            contextFixture.Context, 
            new StorageService(ConfigurationHelper.Configuration), 
            ConfigurationHelper.Configuration);
        var request = new UploadFilesRequest([
            new UploadFileItem("my_file1.txt", 1_000_000), 
            new UploadFileItem("my_file2.png", 1_000_000), 
            new UploadFileItem("my_file3.pdf", 1_000_000),
            new UploadFileItem("my_file4", 1_000_000),
        ]);
        
        var result = await handler.GetSignedUrlForFilesAsync(request);
        var actualDbData = await contextFixture.Context.UserFiles.ToListAsync();
        
        result.IsSuccess.Should().Be(true);
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(request.Files.Count);
        result.Data.All(f => !string.IsNullOrWhiteSpace(f.Url))
            .Should().BeTrue("all urls should have been returned");
        result.Data
            .All(f => actualDbData.Exists(uf => string.Equals(f.OriginalFileName, uf.GetFileNameWithExtension())))
            .Should().BeTrue("all requested files should have been created into db");
    }
    
    [Fact]
    public async Task GetSignedUrlForFilesAsync_RepeatedFileName_ReturnsBadRequest()
    {
        var handler = new FileUploaderHandler(
            contextFixture.Context, 
            new StorageService(ConfigurationHelper.Configuration), 
            ConfigurationHelper.Configuration);
        var request = new UploadFilesRequest([
            new UploadFileItem("my_file1.txt", 1_000_000), 
            new UploadFileItem("my_file1.txt", 1_000_000), 
            new UploadFileItem("my_file3.pdf", 1_000_000)
        ]);
        
        var result = await handler.GetSignedUrlForFilesAsync(request);

        result.IsSuccess.Should().Be(false);
        result.Code.Should().Be(HttpStatusCode.BadRequest);
        result.Data.Should().BeNull(); 
    }
    
    [Theory]
    [InlineData(".")]
    [InlineData(".png")]
    [InlineData("itTkfrVPprmtbNnpbnFUraiJrxGyZNEZjbQKBnaWnGQajyRTmTtNBWFSGjPxTTFRz.exe")]
    [InlineData("helloworld.aaabbbcccddd")]
    public async Task GetSignedUrlForFilesAsync_InvalidFileName_ReturnsBadRequest(string invalidFileName)
    {
        var handler = new FileUploaderHandler(
            contextFixture.Context, new StorageService(
                ConfigurationHelper.Configuration), ConfigurationHelper.Configuration);
        var request = new UploadFilesRequest([new UploadFileItem(invalidFileName, 100)]);
        
        var result = await handler.GetSignedUrlForFilesAsync(request);

        result.IsSuccess.Should().Be(false);
        result.Code.Should().Be(HttpStatusCode.BadRequest);
        result.Data.Should().BeNull(); 
    }
}