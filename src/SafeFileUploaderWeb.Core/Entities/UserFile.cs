namespace SafeFileUploaderWeb.Core.Entities;

public class UserFile
{
    public long Id { get; set; }
    public long FileSize { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string GetFileBucketName() => $"{Id}{FileName}{Extension}";
    
    public string GetFileNameWithExtension() => $"{FileName}{Extension}";
}
