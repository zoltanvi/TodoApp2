using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Common;
using Modules.Common.Database;
using Modules.Notes.Repositories.Models;

namespace Modules.Notes.Repositories;

public class NotesDbContext : DbContext
{
    public DbSet<Note> Notes { get; set; }

    public DbSet<NotesDbInfo> NotesDbInfo { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(DbConfiguration.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var dateTimeConverter = new ValueConverter<DateTime, string>(
            v => v.ToString(Constants.SortableDateFormat),
            v => DateTime.ParseExact(v, Constants.SortableDateFormat, null)
        );

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
            .Property(e => e.Title)
            .IsRequired();

            entity
            .Property(e => e.ListOrder)
            .HasDefaultValue(0);

            entity
            .Property(e => e.CreationDate)
            .HasConversion(dateTimeConverter);

            entity
            .Property(e => e.ModificationDate)
            .HasConversion(dateTimeConverter);

            entity
            .Property(e => e.IsDeleted)
            .IsRequired();
        });

        modelBuilder.Entity<NotesDbInfo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
            .Property(e => e.Initialized)
            .HasDefaultValue(false);
        });
    }
}
