namespace SafeFileUploaderWeb.Core.Entities;

public class UserFile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public long FileSize { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty; // it must store the period ('.')
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Returns in the format {Id}.{Extension}
    /// </summary>
    /// <returns></returns>
    public string GetFileBucketName() => $"{Id}{Extension}";
    
    public string GetFileNameWithExtension() => $"{FileName}{Extension}";
}
