using System.Collections.Generic;

namespace TodoApp2.Core
{
    /// <summary>
    /// A view model the task list item on the task page
    /// </summary>
    public class TaskListViewModel : BaseViewModel
    {
        /// <summary>
        /// The task list items for the list
        /// </summary>
        public List<TaskListItemViewModel> Items { get; set; }
    }
}
