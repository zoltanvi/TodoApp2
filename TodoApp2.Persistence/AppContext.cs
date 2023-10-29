using System.Collections.Generic;
using System.Linq;
using TodoApp2.Entity;
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
        public DbSet<Setting> Settings { get; }
        public DbSet<Category> Categories { get; }
        public DbSet<Note> Notes { get; }
        public DbSet<Task> Tasks { get; }

        public AppContext(string connectionString) : base(connectionString)
        {
            Settings = new DbSet<Setting>(Connection, new SettingsTableMapping());
            Categories = new DbSet<Category>(Connection, new CategoryTableMapping());
            Notes = new DbSet<Note>(Connection, new NoteTableMapping());
            Tasks = new DbSet<Task>(Connection, new TaskTableMapping());
        }
    }
}
