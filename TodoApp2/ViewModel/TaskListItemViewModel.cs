using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp2.ViewModel;

namespace TodoApp2
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
        public int Trashed { get; set; }
        public int ReminderId { get; set; }
    }
}
