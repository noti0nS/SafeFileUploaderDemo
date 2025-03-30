using Microsoft.Extensions.Configuration;

namespace SafeFileUploader.Api.Testing.Helpers;

public static class ConfigurationHelper
{
    public static IConfiguration Configuration { get; }

    static ConfigurationHelper()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.testing.json", optional: false, reloadOnChange: true)
            .Build();
    }
    
    public static void SetStorageUrl(string storageUrl)
      => Configuration["Google:StorageUrl"] = storageUrl;

}