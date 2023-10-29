using TodoApp2.Entity;
using TodoApp2.Persistence.Migrations;
using TodoApp2.Persistence.Models;
using TodoApp2.Persistence.TableMappings;
using Task = TodoApp2.Persistence.Models.Task;

namespace TodoApp2.Persistence
{
    /// <summary>
    /// The Application context through which the database can be managed.
    /// </summary>
    public class AppContext : DbContext
    {
        public DbSet<Setting> Settings { get; private set; }
        public DbSet<Category> Categories { get; private set; }
        public DbSet<Note> Notes { get; private set; }
        public DbSet<Task> Tasks { get; private set; }

        public AppContext(string connectionString) : base(connectionString)
        {
        }

        public override void CreateDbSets()
        {
            Settings = new DbSet<Setting>(Connection, new SettingsTableMapping());
            Categories = new DbSet<Category>(Connection, new CategoryTableMapping());
            Notes = new DbSet<Note>(Connection, new NoteTableMapping());
            Tasks = new DbSet<Task>(Connection, new TaskTableMapping());
        }

        public override void AddMigrations()
        {
            AddMigration(new DbMigration2());
            AddMigration(new DbMigration3());
            AddMigration(new DbMigration4());
            AddMigration(new DbMigration5());
        }
    }
}
