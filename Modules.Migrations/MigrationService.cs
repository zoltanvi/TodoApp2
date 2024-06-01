using Microsoft.EntityFrameworkCore;
using Modules.Notes.Repositories;
using Modules.Settings.Repositories;

namespace Modules.Migrations
{
    public class MigrationService : IMigrationService
    {
        private SettingDbContext _settingContext;
        private NotesDbContext _notesContext;

        public MigrationService(
            SettingDbContext settingsContext,
            NotesDbContext notesContext)
        {
            ArgumentNullException.ThrowIfNull(settingsContext);
            ArgumentNullException.ThrowIfNull(notesContext);

            _settingContext = settingsContext;
            _notesContext = notesContext;
        }

        public void Run()
        {
            RunMigrations(_settingContext);
            RunMigrations(_notesContext);
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
