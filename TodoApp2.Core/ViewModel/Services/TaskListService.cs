using System.Collections.ObjectModel;

namespace TodoApp2.Core
{
    /// <summary>
    /// Service to hold the task item list.
    /// Because this is a service, it can be accessed from multiple ViewModels.
    /// </summary>
    public class TaskListService : BaseViewModel
    {
        private ApplicationSettings ApplicationSettings => IoC.Application.ApplicationSettings;

        /// <summary>
        /// The task list items
        /// </summary>
        public ObservableCollection<TaskListItemViewModel> Items { get; set; }

        public TaskListService()
        {
            Items = new ObservableCollection<TaskListItemViewModel>();
        }
    }
}
