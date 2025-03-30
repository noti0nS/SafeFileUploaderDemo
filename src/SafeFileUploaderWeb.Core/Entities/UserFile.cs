namespace SafeFileUploaderWeb.Core.Entities;

public class UserFile
{
    public long Id { get; set; }
    public long FileSize { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty; // it must store the period ('.')
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Returns in the format {Id}_{FileName}.{Extension}
    /// </summary>
    /// <returns></returns>
    public string GetFileBucketName() => $"{Id}_{FileName}{Extension}";
    
    public string GetFileNameWithExtension() => $"{FileName}{Extension}";
}
