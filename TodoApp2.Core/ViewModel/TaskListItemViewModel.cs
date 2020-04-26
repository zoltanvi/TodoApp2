﻿namespace TodoApp2.Core
{
    /// <summary>
    /// A view model for each task list item on the task page
    /// </summary>
    public class TaskListItemViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public int ListOrder { get; set; }
        public bool IsDone { get; set; }
        public long CreationDate { get; set; }
        public long ModificationDate { get; set; }
        public string Color { get; set; }
        public bool Trashed { get; set; }
        public int ReminderId { get; set; }
    }
}
