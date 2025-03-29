namespace SafeFileUploaderWeb.Core;

public static class Constants
{
    private const int ONE_MB = 1024 * 1024;

    public const int MAX_FILE_SIZE_MB = 128;
    public const int MAX_FILE_SIZE_BYTES = MAX_FILE_SIZE_MB * ONE_MB;
}
