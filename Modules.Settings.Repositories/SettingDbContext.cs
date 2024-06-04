using Microsoft.EntityFrameworkCore;
using Modules.Common.Database;
using Modules.Settings.Contracts.Models;

namespace Modules.Settings.Repositories;

public class SettingDbContext : DbContext
{
    public DbSet<Setting> Settings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(DbConfiguration.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Key);
        });
    }
}
