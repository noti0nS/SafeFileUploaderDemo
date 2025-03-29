namespace SafeFileUploaderWeb.Core;

public static class Constants
{
    private const int OneMb = 1024 * 1024;

    public const int MaxFileSizeMb = 128;
    public const int MaxFileSizeBytes = MaxFileSizeMb * OneMb;
    
    public const int MaxFileNameLength = 64;
    public const int MaxExtensionLength = 8;
}
