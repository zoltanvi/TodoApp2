using TodoApp2.Entity;
using TodoApp2.Persistence.Models;
using Task = TodoApp2.Persistence.Models.Task;

namespace TodoApp2.Persistence
{
    /// <summary>
    /// The Application context through which the database can be managed.
    /// </summary>
    public class AppContext : DbContext
    {
        public AppContext(string connectionString) : base(connectionString)
        {
            Settings = new DbSet<Setting>(Connection, "Settings");
            Categories = new DbSet<Category>(Connection, "Category");
            Notes = new DbSet<Note>(Connection, "Note");
            Tasks = new DbSet<Task>(Connection, "Task");
        }

        public DbSet<Setting> Settings { get; }
        public DbSet<Category> Categories { get; }
        public DbSet<Note> Notes { get; }
        public DbSet<Task> Tasks { get; }
    }
}
