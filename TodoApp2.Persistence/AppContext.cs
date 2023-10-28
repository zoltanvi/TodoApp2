using System.Collections.Generic;
using System.Linq;
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
        public DbSet<Setting> Settings { get; }
        public DbSet<Category> Categories { get; }
        public DbSet<Note> Notes { get; }
        public DbSet<Task> Tasks { get; }

        public AppContext(string connectionString) : base(connectionString)
        {
            Settings = new DbSet<Setting>(Connection, "Settings");
            Categories = new DbSet<Category>(Connection, "Category");
            Notes = new DbSet<Note>(Connection, "Note");
            Tasks = new DbSet<Task>(Connection, "Task");

            ConfigureTables();
            CreateTables();
        }

        private void ConfigureTables()
        {
            string defaultListOrder = (long.MaxValue / 2).ToString();
            string transparent = "Transparent";
            string zero = "0";

            Categories.Configure()
               .Property(x => x.Id, true)
               .Property(x => x.Name)
               .Property(x => x.ListOrder, defaultListOrder)
               .Property(x => x.Trashed);

            Tasks.Configure()
                .Property(x => x.Id, true)
                .Property(x => x.CategoryId)
                .Property(x => x.Content)
                .Property(x => x.ListOrder, defaultListOrder)
                .Property(x => x.Pinned, zero)
                .Property(x => x.IsDone, zero)
                .Property(x => x.CreationDate)
                .Property(x => x.ModificationDate)
                .Property(x => x.Color, transparent)
                .Property(x => x.BorderColor, transparent)
                .Property(x => x.BackgroundColor, transparent)
                .Property(x => x.Trashed, zero)
                .Property(x => x.ReminderDate, zero)
                .Property(x => x.IsReminderOn, zero)
                .ForeignKey<Category>(task => task.CategoryId, category => category.Id);

            Settings.Configure()
                .Property(x => x.Key, true)
                .Property(x => x.Value);

            Notes.Configure()
                .Property(x => x.Id, true)
                .Property(x => x.Title)
                .Property(x => x.ListOrder, defaultListOrder)
                .Property(x => x.Content)
                .Property(x => x.CreationDate)
                .Property(x => x.ModificationDate)
                .Property(x => x.Trashed, zero);
        }

        private void CreateTables()
        {
            Categories.CreateIfNotExists();
            Tasks.CreateIfNotExists();
            Settings.CreateIfNotExists();
            Notes.CreateIfNotExists();
        }
    }
}
