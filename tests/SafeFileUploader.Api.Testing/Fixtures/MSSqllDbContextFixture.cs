using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SafeFileUploader.Api.Testing.Helpers;
using SafeFileUploaderWeb.Api.Data;

namespace SafeFileUploader.Api.Testing.Fixtures;

public class MsSqlDbContextFixture : IAsyncLifetime
{    
    public DatabaseContext Context { get; private set; }

    public MsSqlDbContextFixture() 
    {
        var connString = ConfigurationHelper.GetConfiguration().GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>()
           .EnableSensitiveDataLogging()
           .EnableDetailedErrors()
           .UseSqlServer(connString);
        Context = new DatabaseContext(optionsBuilder.Options);
    }

    public Task InitializeAsync()
     => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await CleanUpDataAsync();
        await Context.DisposeAsync();
    }

    public async Task CleanUpDataAsync()
    {
        Context.ChangeTracker.Clear();
        await Context.UserFiles.ExecuteDeleteAsync();
    }
}
