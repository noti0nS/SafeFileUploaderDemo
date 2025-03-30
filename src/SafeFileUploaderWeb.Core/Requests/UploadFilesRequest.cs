using FluentValidation;

namespace SafeFileUploaderWeb.Core.Requests;

public record UploadFilesRequest(List<UploadFileItem> Files)
{
    public string? Validate()
      => new UploadFilesRequestValidator().Validate(this)
          .Errors
          .FirstOrDefault()
          ?.ErrorMessage;
}

public record UploadFileItem(string Name, long FileSizeBytes);

public class UploadFilesRequestValidator : AbstractValidator<UploadFilesRequest>
{
    public UploadFilesRequestValidator()
    {
        RuleFor(x => x.Files)
            .NotEmpty().WithMessage("No files were provided.")
            .Must(NotContainDuplicates).WithMessage("Files with the same name are not allowed.");
        RuleForEach(x => x.Files).SetValidator(new UploadFileItemValidator());
    }
    
    private static bool NotContainDuplicates(IEnumerable<UploadFileItem>? collection)
    {
        return collection == null || 
               collection.GroupBy(f => f.Name).All(g => g.Count() == 1);
    }
}

internal class UploadFileItemValidator : AbstractValidator<UploadFileItem>
{
    public UploadFileItemValidator()
    {
        RuleFor(x => Path.GetFileNameWithoutExtension(x.Name))
            .NotEmpty().WithMessage("The file name cannot be empty.")
            .MaximumLength(Constants.MaxFileNameLength).WithMessage(f => $"{f.Name}: The file name is too long. It must be until {Constants.MaxFileNameLength} characters.")
            .WithName(nameof(UploadFileItem.Name));
        RuleFor(x => Path.GetExtension(x.Name))
            .MaximumLength(Constants.MaxExtensionLength).WithMessage(f => $"{f.Name}: The file extension cannot be greater than {Constants.MaxExtensionLength} characters.")
            .WithName(nameof(UploadFileItem.Name));;
        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0).WithMessage("Invalid file size.")
            .LessThanOrEqualTo(Constants.MaxFileSizeBytes).WithMessage(f =>  $"{f.Name}: The file size cannot be greater than {Constants.MaxFileSizeMb} MB.");
    }
}