using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp2.ViewModel;

namespace TodoApp2
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
