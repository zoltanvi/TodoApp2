using Microsoft.EntityFrameworkCore;

namespace Modules.Migrations
{
    public class MigrationService : IMigrationService
    {
        public void Run(IEnumerable<DbContext> contexts)
        {
            ArgumentNullException.ThrowIfNull(contexts);

            if (!contexts.Any())
            {
                throw new ArgumentNullException(nameof(contexts));
            }

            foreach (var context in contexts)
            {
                RunMigrations(context);
            }
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
