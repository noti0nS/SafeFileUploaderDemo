namespace SafeFileUploaderWeb.Api.Configuration;

public class GoogleOptionsConfig
{
    public const string Section = "Google";

    public string ProjectId { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public string PrivateKey { get; set; } = string.Empty;
    public string Bucket { get; set; } = string.Empty;
    public string StorageUrl { get; set; } = string.Empty;
    public int PreSignedUrlDurationInMinutes { get; set; }
}
