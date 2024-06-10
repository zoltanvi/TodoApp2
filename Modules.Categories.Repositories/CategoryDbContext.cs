using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Categories.Contracts.Models;
using Modules.Common;
using Modules.Common.Database;

namespace Modules.Categories.Repositories;

public class CategoryDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }

    public DbSet<CategoriesDbInfo> CategoriesDbInfo { get; set; }

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

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
            .Property(e => e.Name)
            .IsRequired();

            entity
            .HasIndex(e => e.Name)
            .IsUnique();

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

        modelBuilder.Entity<CategoriesDbInfo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity
            .Property(e => e.Initialized)
            .HasDefaultValue(false);
        });
    }
}
