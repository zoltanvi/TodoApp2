using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

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
        public ObservableCollection<TaskListItemViewModel> Items { get; set; }

        /// <summary>
        /// The content / description text for the current task being written
        /// </summary>
        public string PendingAddNewTaskText { get; set; }

        private string CurrentCategory => IoC.Get<ApplicationViewModel>().CurrentCategory;
        private ClientDatabase Database => IoC.Get<ClientDatabase>();

        /// <summary>
        /// The command for when the user presses Enter in the "Add new task" TextBox
        /// </summary>
        public ICommand AddTaskItemCommand { get; }

        public ICommand DeleteTaskItemCommand { get; }


        public TaskListViewModel()
        {
            AddTaskItemCommand = new RelayCommand(AddTask);

            DeleteTaskItemCommand = new RelayParameterizedCommand(TrashTask);

            // Query the items with the current category
            List<TaskListItemViewModel> items = Database.GetActiveTaskItems(CurrentCategory);

            // Fill the actual list with the queried items
            Items = new ObservableCollection<TaskListItemViewModel>(items);

            Items.CollectionChanged += ItemsOnCollectionChanged;
        }

        private void TrashTask(object obj)
        {
            if (obj is TaskListItemViewModel task)
            {
                Database.TrashTask(task);

                Items.Remove(task);
            }
        }

        /// <summary>
        /// Persists the Items list into the database
        /// </summary>
        public void PersistTaskList()
        {
            Database.UpdateTaskList(Items);
        }

        /// <summary>
        /// Called when the Items collection changes.
        /// We want to synchronize the list order when it happens.
        /// Note: The Drag & Drop causes a Remove and Add action sequence.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                {
                    // Persist the reordered list into database
                    Database.UpdateTaskListOrders(Items);
                    break;
                } 
            }
        }

        private void AddTask()
        {
            // Create the new task instance
            TaskListItemViewModel taskToAdd = new TaskListItemViewModel
            {
                Category = CurrentCategory,
                Content = PendingAddNewTaskText,
            };

            // Persist into database and set the task ID
            Database.AddTask(taskToAdd);

            // Add the task into the ViewModel list 
            Items.Insert(0, taskToAdd);

            // Reset the input TextBox text
            PendingAddNewTaskText = string.Empty;
        }

    }
}
