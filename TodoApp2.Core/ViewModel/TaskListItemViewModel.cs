using System;

namespace TodoApp2.Core
{
    /// <summary>
    /// A view model for each task list item on the task page
    /// </summary>
    public class TaskListItemViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public int ListOrder { get; set; } = 0;
        public bool IsDone { get; set; } = false;
        public long CreationDate { get; set; } = DateTime.Now.Ticks;
        public long ModificationDate { get; set; } = DateTime.Now.Ticks;
        public string Color { get; set; } = string.Empty;
        public bool Trashed { get; set; } = false;
        public int? ReminderId { get; set; } = null;
    }
}
