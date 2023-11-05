using System;
using TodoApp2.Entity;
using TodoApp2.Persistence.Models;
using Task = TodoApp2.Persistence.Models.Task;

namespace TodoApp2.Persistence
{
    public interface IAppContext : IDisposable
    {
        DbSet<Setting> Settings { get; }
        DbSet<Category> Categories { get; }
        DbSet<Note> Notes { get; }
        DbSet<Task> Tasks { get; }
    }
}
