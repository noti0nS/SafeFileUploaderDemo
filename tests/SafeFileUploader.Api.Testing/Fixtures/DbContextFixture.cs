using Microsoft.EntityFrameworkCore;
using SafeFileUploaderWeb.Api.Data;
using Testcontainers.PostgreSql;

namespace SafeFileUploader.Api.Testing.Fixtures;

public class DbContextFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;

    public DatabaseContext Context { get; private set; } = null!;

    public DbContextFixture()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres@sha256:6cf6142afacfa89fb28b894d6391c7dcbf6523c33178bdc33e782b3b533a9342")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>()
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .UseNpgsql(_container.GetConnectionString());
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
