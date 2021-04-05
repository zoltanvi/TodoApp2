using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TodoApp2.Core
{
    /// <summary>
    /// Service to hold the task item list.
    /// Because this is a service, it can be accessed from multiple ViewModels.
    /// </summary>
    public class TaskListService : BaseViewModel
    {
        private string CurrentCategory => IoC.CategoryListService.CurrentCategory;
        private ClientDatabase Database => IoC.ClientDatabase;

        /// <summary>
        /// The task list items
        /// </summary>
        public ObservableCollection<TaskListItemViewModel> TaskPageItems { get; }

        public TaskListService()
        {
            // Query the items with the current category
            List<TaskListItemViewModel> items = Database.GetActiveTaskItems(CurrentCategory);

            // Fill the actual list with the queried items
            TaskPageItems = new ObservableCollection<TaskListItemViewModel>(items);

            IoC.ClientDatabase.TaskChanged += OnClientDatabaseTaskChanged;
        }

        private void OnClientDatabaseTaskChanged(object sender, TaskChangedEventArgs e)
        {
            TaskListItemViewModel modifiedItem = TaskPageItems.FirstOrDefault(item => item.Id == e.Task.Id);
            
            modifiedItem?.CopyProperties(e.Task);
        }
    }
}
