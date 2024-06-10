using TodoApp2.Common;
using TodoApp2.Entity;
using TodoApp2.Persistence.Models;
using Task = TodoApp2.Persistence.Models.Task;

namespace TodoApp2.Persistence.TableMappings
{
    internal class TaskTableMapping : DbSetMapping<Task>
    {
        public TaskTableMapping() : base(Constants.TableNames.Task)
        {
            ModelBuilder
                .Property(x => x.Id, isPrimaryKey: true)
                .Property(x => x.CategoryId)
                .Property(x => x.Content)
                .Property(x => x.ListOrder, defaultValue: CommonConstants.DefaultListOrderString)
                .Property(x => x.Pinned, defaultValue: Constants.Zero)
                .Property(x => x.IsDone, defaultValue: Constants.Zero)
                .Property(x => x.CreationDate)
                .Property(x => x.ModificationDate)
                .Property(x => x.Color, defaultValue: Constants.Transparent)
                .Property(x => x.BorderColor, defaultValue: Constants.Transparent)
                .Property(x => x.BackgroundColor, defaultValue: Constants.Transparent)
                .Property(x => x.Trashed, defaultValue: Constants.Zero)
                .Property(x => x.TrashedDate, defaultValue: Constants.Zero)
                .Property(x => x.ReminderDate, defaultValue: Constants.Zero)
                .Property(x => x.IsReminderOn, defaultValue: Constants.Zero);
        }
    }
}
