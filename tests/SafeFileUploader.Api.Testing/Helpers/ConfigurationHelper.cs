using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace SafeFileUploader.Api.Testing.Helpers;

public static class ConfigurationHelper
{
    private static readonly IConfiguration _configuration;

    static ConfigurationHelper()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.testing.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public static IOptions<T> GetOptions<T>(string section)
        where T : class, new()
    {
        var options = new T();
        _configuration.GetSection(section).Bind(options);
        return Options.Create(options);
    }
}