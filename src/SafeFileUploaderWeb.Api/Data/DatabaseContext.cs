using Microsoft.EntityFrameworkCore;
using SafeFileUploaderWeb.Core;
using SafeFileUploaderWeb.Core.Entities;

namespace SafeFileUploaderWeb.Api.Data;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) 
    : DbContext(options)
{
    public DbSet<UserFile> UserFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserFile>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.FileSize)
                .IsRequired();
            b.Property(p => p.FileName)
                .IsRequired()
                .HasMaxLength(Constants.MaxFileNameLength);
            b.Property(p => p.Extension)
                .IsRequired()
                .HasDefaultValue(string.Empty)
                .HasMaxLength(Constants.MaxExtensionLength);
            b.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("timezone('utc', now())");
        });
    }
}
