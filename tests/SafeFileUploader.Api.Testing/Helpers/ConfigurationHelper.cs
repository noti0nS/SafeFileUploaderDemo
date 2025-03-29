using Microsoft.Extensions.Configuration;

namespace SafeFileUploader.Api.Testing.Helpers;

public static class ConfigurationHelper
{
    public static IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Ensure the path is correct
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }
}