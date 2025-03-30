using System.Text;
using FluentAssertions;
using FluentValidation.TestHelper;
using SafeFileUploaderWeb.Core;
using SafeFileUploaderWeb.Core.Requests;

namespace SafeFileUploader.Api.Testing.Tests.UnitTesting;

public class UploadFilesRequestValidatorTest
{
    private readonly UploadFilesRequestValidator _sut = new();
    
    [Theory]
    [InlineData(".")]
    [InlineData(".png")]
    public async Task InvalidFileName_ReturnsFailure(string fileName)
    {
        var request = new UploadFilesRequest([new UploadFileItem(fileName, 1_000_000)]);
        
        var result = await _sut.TestValidateAsync(request);
        
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(BuildPropertyPath(nameof(UploadFileItem.Name)));
    }
    
    [Fact]
    public async Task TooLongFileName_ReturnsFailure()
    {
        var fileName = new StringBuilder();
        for (int i = 0; i < Constants.MaxFileNameLength + 1; i++)
            fileName.Append('a');
        fileName.Append(".exe");
        var request = new UploadFilesRequest([new UploadFileItem(fileName.ToString(), 1_000_000)]);
        
        var result = await _sut.TestValidateAsync(request);
        
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(BuildPropertyPath(nameof(UploadFileItem.Name)));
    }
    
    [Fact]
    public async Task TooLongExtension_ReturnsFailure()
    {
        var fileName = new StringBuilder("hello_world.");
        for (int i = 0; i < Constants.MaxExtensionLength + 1; i++) 
            fileName.Append('a');
        
        var request = new UploadFilesRequest([new UploadFileItem(fileName.ToString(), 1_000_000)]);
        
        var result = await _sut.TestValidateAsync(request);
        
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(BuildPropertyPath(nameof(UploadFileItem.Name)));
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task InvalidFileSize_ReturnsFailure(long fileSizeInBytes)
    {
        var request = new UploadFilesRequest([new UploadFileItem("example.txt", fileSizeInBytes)]);
        
        var result = await _sut.TestValidateAsync(request);
        
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(BuildPropertyPath(nameof(UploadFileItem.FileSizeBytes)));
    }
    
    [Fact]
    public async Task HeavyFile_ReturnsFailure()
    {
        var request = new UploadFilesRequest([new UploadFileItem("example.txt", Constants.MaxFileSizeBytes + 1)]);
        
        var result = await _sut.TestValidateAsync(request);
        
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(BuildPropertyPath(nameof(UploadFileItem.FileSizeBytes)));
    }

    [Fact]
    public async Task RepeatedFileNames_ReturnsFailure()
    {
        var request = new UploadFilesRequest([
            new UploadFileItem("example.txt", Constants.MaxFileSizeBytes),
            new UploadFileItem("example.txt", Constants.MaxFileSizeBytes),
            new UploadFileItem("example.pdf", Constants.MaxFileSizeBytes),
        ]);
        
        var result = await _sut.TestValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Files);
    }
    
    /// <summary>
    /// Build string following the pattern "Files[<paramref name="index"/>].<paramref name="propertyName"></paramref>"
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static string BuildPropertyPath(string propertyName, int index = 0)
      => $"{nameof(UploadFilesRequest.Files)}[{index}].{propertyName}";
}