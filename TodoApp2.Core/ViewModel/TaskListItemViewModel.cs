using System;

namespace TodoApp2.Core
{
    /// <summary>
    /// A view model for each task list item on the task page
    /// </summary>
    public class TaskListItemViewModel : BaseViewModel, IReorderable
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Content { get; set; }
        public long ListOrder { get; set; }
        public bool IsDone { get; set; }
        public long CreationDate { get; set; } = DateTime.Now.Ticks;
        public long ModificationDate { get; set; } = DateTime.Now.Ticks;
        public string Color { get; set; }
        public bool Trashed { get; set; }
        public long ReminderDate { get; set; }
        public bool IsReminderOn { get; set; }
    }
}
