namespace SafeFileUploaderWeb.Core.Requests;

public record UploadFilesRequest(List<UploadFileItem> Files);

public record UploadFileItem(string Name, long FileSizeBytes);