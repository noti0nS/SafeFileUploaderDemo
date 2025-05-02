using Microsoft.EntityFrameworkCore;
using SafeFileUploaderWeb.Api.Data;
using Testcontainers.MsSql;

namespace SafeFileUploader.Api.Testing.Fixtures;

public class MsSqlDbContextFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container;

    public DatabaseContext Context { get; private set; } = null!;

    public MsSqlDbContextFixture()
    {
        _container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server@sha256:ea73825f3d88a23c355ac2f9fdc6bd960fec90171c12c572109b36a558f77bb8")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>()
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .UseSqlServer(_container.GetConnectionString());
        Context = new DatabaseContext(optionsBuilder.Options);
        await Context.Database.MigrateAsync();
        await Context.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync()
        => Task.CompletedTask;

    public async Task CleanUpDataAsync()
    {
        Context.ChangeTracker.Clear();
        await Context.UserFiles.ExecuteDeleteAsync();
    }
}
