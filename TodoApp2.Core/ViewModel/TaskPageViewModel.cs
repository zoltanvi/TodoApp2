using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TodoApp2.Core
{
    /// <summary>
    /// A view model the task list item on the task page
    /// </summary>
    public class TaskPageViewModel : BaseViewModel
    {
        private int m_LastRemovedId = int.MinValue;
        private string CurrentCategory => IoC.CategoryListService.CurrentCategory;
        private ClientDatabase Database => IoC.ClientDatabase;

        /// <summary>
        /// The task list items
        /// </summary>
        public ObservableCollection<TaskListItemViewModel> Items { get; }

        /// <summary>
        /// The content / description text for the current task being written
        /// </summary>
        public string PendingAddNewTaskText { get; set; }

        /// <summary>
        /// The command for when the user presses Enter in the "Add new task" TextBox
        /// </summary>
        public ICommand AddTaskItemCommand { get; }

        /// <summary>
        /// The command for when the trash button is pressed in the task item
        /// </summary>
        public ICommand DeleteTaskItemCommand { get; }

        /// <summary>
        /// The command for when the done checkbox is checked in the task item
        /// </summary>
        public ICommand TaskIsDoneModifiedCommand { get; }

        /// <summary>
        /// The command to move a task item into another category
        /// </summary>
        public ICommand MoveToCategoryCommand { get; }

        public TaskPageViewModel()
        {
            AddTaskItemCommand = new RelayCommand(AddTask);
            DeleteTaskItemCommand = new RelayParameterizedCommand(TrashTask);
            TaskIsDoneModifiedCommand = new RelayParameterizedCommand(ModifyTaskIsDone);
            MoveToCategoryCommand = new RelayParameterizedCommand(MoveToCategory);

            // Load the application settings to update the CurrentCategory before querying the tasks
            IoC.Application.LoadApplicationSettingsOnce();

            // Query the items with the current category
            List<TaskListItemViewModel> items = Database.GetActiveTaskItems(CurrentCategory);

            // Fill the actual list with the queried items
            Items = new ObservableCollection<TaskListItemViewModel>(items);

            // Subscribe to the collection changed event for synchronizing with database
            Items.CollectionChanged += ItemsOnCollectionChanged;

            // Subscribe to the category changed event to filter the list when it happens
            Mediator.Instance.Register(OnCategoryChanged, ViewModelMessages.CategoryChanged);

            // Subscribe to the theme changed event to repaint the list items when it happens
            Mediator.Instance.Register(OnThemeChanged, ViewModelMessages.ThemeChanged);
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
        /// Moves a task into another category
        /// </summary>
        /// <param name="obj"></param>
        private void MoveToCategory(object obj)
        {
            if (obj is List<object> parameters && parameters.Count == 2)
            {
                if (parameters[0] is TaskListItemViewModel task &&
                    parameters[1] is string categoryToMoveTo)
                {
                    CategoryListItemViewModel taskCategory = Database.GetCategory(task.CategoryId);

                    // If the category is the same as the task is in, there is nothing to do
                    if (taskCategory.Name != categoryToMoveTo)
                    {
                        CategoryListItemViewModel newCategory = Database.GetCategory(categoryToMoveTo);
                        task.CategoryId = newCategory.Id;

                        // Insert into first position.
                        // The modified category also gets persisted.
                        Database.ReorderTask(task, 0);

                        // Delete the item from the currently listed category items at last
                        Items.Remove(task);
                    }
                }
            }
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
                {
                    if (e.NewItems.Count > 0)
                    {
                        var newItem = (TaskListItemViewModel)e.NewItems[0];

                        // If the newly added item is the same as the last deleted one,
                        // then this was a drag and drop reorder
                        if (newItem.Id == m_LastRemovedId)
                        {
                            Database.ReorderTask(newItem, e.NewStartingIndex);
                        }

                        m_LastRemovedId = int.MinValue;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems.Count > 0)
                    {
                        var last = (TaskListItemViewModel)e.OldItems[0];

                        m_LastRemovedId = last.Id;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Move:
                {
                    if (e.NewItems.Count > 0)
                    {
                        var newItem = (TaskListItemViewModel)e.NewItems[0];
                        Database.ReorderTask(newItem, e.NewStartingIndex);
                    }
                    break;
                }
            }
        }

        public void AddTask()
        {
            // If the text is empty or only whitespace, refuse
            // If the text only contains format characters, refuse
            string trimmed = PendingAddNewTaskText?.Replace("`", string.Empty);
            if (string.IsNullOrWhiteSpace(PendingAddNewTaskText) || string.IsNullOrWhiteSpace(trimmed))
            {
                return;
            }

            // Create the new task instance
            TaskListItemViewModel taskToAdd = new TaskListItemViewModel
            {
                CategoryId = Database.GetCategory(CurrentCategory).Id,
                Content = PendingAddNewTaskText,
                CreationDate = DateTime.Now.Ticks,
                ModificationDate = DateTime.Now.Ticks
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

        private async Task OnCategoryChanged()
        {
            // Clear the list first to prevent inconsistent data on UI while the items are loading
            Items.Clear();

            // Query the items with the current category
            List<TaskListItemViewModel> filteredItems = await Database.GetActiveTaskItemsAsync(CurrentCategory);

            // Clear the list to prevent showing items from multiple categories.
            // This can happen if the user changes category again while the query runs
            Items.Clear();

            // Fill the actual list with the queried items
            Items.AddRange(filteredItems);
        }

        /// <summary>
        /// Forces the UI to repaint the list items when the theme changes
        /// </summary>
        /// <param name="obj"></param>
        private void OnThemeChanged(object obj)
        {
            // Save the current items
            List<TaskListItemViewModel> itemsBackup = new List<TaskListItemViewModel>(Items);

            // Clear the items and add back the cleared items to refresh the list (repaint)
            Items.Clear();
            Items.AddRange(itemsBackup);
        }
    }
}