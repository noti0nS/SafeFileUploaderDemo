namespace SafeFileUploaderWeb.Core.Entities;

public class UserFile
{
    public long Id { get; set; }
    public long FileSize { get; set; }
    public string UniqueFileName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
