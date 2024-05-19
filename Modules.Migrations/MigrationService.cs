using Microsoft.EntityFrameworkCore;
using Modules.Settings.Repositories;

namespace Modules.Migrations
{
    public class MigrationService : IMigrationService
    {
        private SettingDbContext _settingContext;

        public MigrationService(SettingDbContext settingDbContext)
        {
            _settingContext = settingDbContext;
        }

        public void Run()
        {
            RunMigrations(_settingContext);
        }

        private void RunMigrations(DbContext dbContext)
        {
            // Get pending migrations
            //dbContext.Database.EnsureCreated();

            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();

            if (pendingMigrations.Any())
            {
                Console.WriteLine("Migrations are pending. Applying...");

                // Apply pending migrations
                dbContext.Database.Migrate();
                Console.WriteLine("Migrations applied successfully.");
            }
        }
    }
}
