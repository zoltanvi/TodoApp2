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

        private string CurrentCategory => IoC.Application.CurrentCategory;
        private bool OptimizePerformance => IoC.Application.OptimizePerformance;
        private ClientDatabase Database => IoC.Get<ClientDatabase>();

        /// <summary>
        /// The command for when the user presses Enter in the "Add new task" TextBox
        /// </summary>
        public ICommand AddTaskItemCommand { get; }

        public ICommand DeleteTaskItemCommand { get; }

        public ICommand TaskIsDoneModifiedCommand { get; }

        public TaskListViewModel()
        {
            AddTaskItemCommand = new RelayCommand(AddTask);

            DeleteTaskItemCommand = new RelayParameterizedCommand(TrashTask);

            TaskIsDoneModifiedCommand = new RelayParameterizedCommand(ModifyTaskIsDone);

            // Query the items with the current category
            List<TaskListItemViewModel> items = Database.GetActiveTaskItems(CurrentCategory);

            // Fill the actual list with the queried items
            Items = new ObservableCollection<TaskListItemViewModel>(items);

            // Subscribe to the collection changed event for synchronizing with database 
            Items.CollectionChanged += ItemsOnCollectionChanged;

            // Subscribe to the category changed event to filter the list when it happens
            Mediator.Instance.Register(OnCategoryChanged, ViewModelMessages.CategoryChanged);
        }


        private void ModifyTaskIsDone(object obj)
        {
            if (obj is TaskListItemViewModel task)
            {
                // If this task is done, move it after the last not done item
                // If this is not done (undone action), move it to the top of the list
                // Because this generates a NotifyCollectionChangedAction.Move action, 
                // all task modifications will be persisted
                if (task.IsDone)
                {
                    MoveTaskToEnd(task);
                }
                else
                {
                    MoveTaskToTop(task);
                }
            }
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
            // This call should never depend on OptimizePerformance property
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
            if (!OptimizePerformance)
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
                    case NotifyCollectionChangedAction.Move:
                    {
                        Database.UpdateTaskList(Items);
                        break;
                    }
                }
            }
        }

        private void AddTask()
        {
            // If the text is empty or only whitespace, refuse
            if (string.IsNullOrWhiteSpace(PendingAddNewTaskText))
            {
                return;
            }

            // Create the new task instance
            TaskListItemViewModel taskToAdd = new TaskListItemViewModel
            {
                CategoryId = Database.GetCategory(CurrentCategory).Id,
                Content = PendingAddNewTaskText,
            };

            // Persist into database and set the task ID
            // This call can't be optimized because the database gives the ID to the task
            Database.AddTask(taskToAdd);

            // Add the task into the ViewModel list 
            Items.Insert(0, taskToAdd);

            // Reset the input TextBox text
            PendingAddNewTaskText = string.Empty;
        }

        private void MoveTaskToEnd(TaskListItemViewModel task)
        {
            int oldIndex = Items.IndexOf(task);
            int newIndex = Items.Count - 1;

            for (int i = Items.Count - 1; i >= 0; i--)
            {
                newIndex = i;
                if (Items[i].Equals(task) || Items[i].IsDone == false)
                {
                    break;
                }
            }

            Items.Move(oldIndex, newIndex);
        }

        private void MoveTaskToTop(TaskListItemViewModel task)
        {
            int oldIndex = Items.IndexOf(task);
            Items.Move(oldIndex, 0);
        }

        private void OnCategoryChanged(object obj)
        {
            // Query the items with the current category
            List<TaskListItemViewModel> filteredItems = Database.GetActiveTaskItems(CurrentCategory);

            // Fill the actual list with the queried items
            Items.Clear();
            Items.AddRange(filteredItems);
        }

    }
}
