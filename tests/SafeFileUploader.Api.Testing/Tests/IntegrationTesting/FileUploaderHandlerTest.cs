using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SafeFileUploader.Api.Testing.Fixtures;
using SafeFileUploaderWeb.Api.Handlers;
using SafeFileUploaderWeb.Core.Requests;

namespace SafeFileUploader.Api.Testing.Tests.IntegrationTesting;

public class FileUploaderHandlerTest 
    : IClassFixture<StorageServiceFixture>, IClassFixture<MsSqlDbContextFixture>, IAsyncLifetime
{
    private readonly MsSqlDbContextFixture _contextFixture;
    private readonly StorageServiceFixture _storageFixture;
    private readonly FileUploaderHandler _handler;

    public FileUploaderHandlerTest(
        MsSqlDbContextFixture contextFixture, StorageServiceFixture storageFixture)
    {
        _contextFixture = contextFixture;
        _storageFixture = storageFixture;
        _handler = new FileUploaderHandler(
            contextFixture.Context, _storageFixture.StorageService, _storageFixture.GoogleOptions);
    }

    Task IAsyncLifetime.InitializeAsync() => Task.CompletedTask;

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _contextFixture.CleanUpDataAsync();
        await _storageFixture.RecreateBucketAsync();
    }
    
    [Fact]
    public async Task GetSignedUrlForFilesAsync_InputValidFiles_OutputSignedUrls()
    {
        var request = new UploadFilesRequest([
            new UploadFileItem("my_file1.txt", 1_000_000), 
            new UploadFileItem("my_file2.png", 1_000_000), 
            new UploadFileItem("my_file3.pdf", 1_000_000),
            new UploadFileItem("my_file4", 1_000_000),
        ]);
        
        var result = await _handler.GetSignedUrlForFilesAsync(request);
        var actualDbData = await _contextFixture.Context.UserFiles.ToListAsync();
        
        result.IsSuccess.Should().Be(true);
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(request.Files.Count);
        result.Data
            .All(f => !string.IsNullOrWhiteSpace(f.Url))
            .Should().BeTrue("all urls should have been returned");
        result.Data
            .All(f => actualDbData.Exists(uf => string.Equals(f.OriginalFileName, uf.GetFileNameWithExtension())))
            .Should().BeTrue("all requested files should have been created into db");
        actualDbData.All(uf => string.IsNullOrEmpty(uf.Extension) || uf.Extension.StartsWith('.'))
            .Should().BeTrue("all saved files must have the Extension property starting with '.' if is not empty.");
    }
}