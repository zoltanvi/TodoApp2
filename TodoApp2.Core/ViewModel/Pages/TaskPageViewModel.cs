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
    public class TaskPageViewModel : BaseViewModel
    {
        private readonly TaskListService m_TaskListService;

        private int m_LastRemovedId = int.MinValue;
        private string CurrentCategory => IoC.CategoryListService.CurrentCategory;
        private ClientDatabase Database => IoC.ClientDatabase;
        private ObservableCollection<TaskListItemViewModel> Items => m_TaskListService.TaskPageItems;
        
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
            m_TaskListService = IoC.Get<TaskListService>();
        }

        public TaskPageViewModel(TaskListService taskListService)
        {
            m_TaskListService = taskListService;

            AddTaskItemCommand = new RelayCommand(AddTask);
            DeleteTaskItemCommand = new RelayParameterizedCommand(TrashTask);
            TaskIsDoneModifiedCommand = new RelayParameterizedCommand(ModifyTaskIsDone);
            MoveToCategoryCommand = new RelayParameterizedCommand(MoveToCategory);

            // Subscribe to the collection changed event for synchronizing with database
            Items.CollectionChanged += ItemsOnCollectionChanged;
            
            // Subscribe to the theme changed event to repaint the list items when it happens
            Mediator.Register(OnThemeChanged, ViewModelMessages.ThemeChanged);
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
                // Set Trashed property to true so it won't be listed in the active list
                task.Trashed = true;

                // Indicate that it is an invalid order
                task.ListOrder = long.MinValue;

                // Persist modifications
                m_TaskListService.UpdateTask(task);

                // Remove from the list
                m_TaskListService.RemoveTask(task);
            }
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
                    // TODO: use CategoryListService call instead
                    CategoryListItemViewModel taskCategory = Database.GetCategory(task.CategoryId);

                    // If the category is the same as the task is in, there is nothing to do
                    if (taskCategory.Name != categoryToMoveTo)
                    {
                        // TODO: use CategoryListService call instead
                        CategoryListItemViewModel newCategory = Database.GetCategory(categoryToMoveTo);
                        task.CategoryId = newCategory.Id;

                        // Insert into first position.
                        // The modification gets persisted
                        m_TaskListService.ReorderTask(task, 0);

                        // Delete the item from the currently listed items
                        m_TaskListService.RemoveTask(task);
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
                        TaskListItemViewModel newItem = (TaskListItemViewModel)e.NewItems[0];

                        // If the newly added item is the same as the last deleted one,
                        // then this was a drag and drop reorder
                        if (newItem.Id == m_LastRemovedId)
                        {
                            m_TaskListService.ReorderTask(newItem, e.NewStartingIndex);
                        }

                        m_LastRemovedId = int.MinValue;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems.Count > 0)
                    {
                        TaskListItemViewModel last = (TaskListItemViewModel)e.OldItems[0];

                        m_LastRemovedId = last.Id;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Move:
                {
                    if (e.NewItems.Count > 0)
                    {
                        TaskListItemViewModel newItem = (TaskListItemViewModel)e.NewItems[0];
                        m_TaskListService.ReorderTask(newItem, e.NewStartingIndex);
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
                // TODO: use CategoryListService call instead
                CategoryId = Database.GetCategory(CurrentCategory).Id,
                Content = PendingAddNewTaskText,
                CreationDate = DateTime.Now.Ticks,
                ModificationDate = DateTime.Now.Ticks
            };

            // Add task to list and persist it
            m_TaskListService.AddNewTask(taskToAdd);

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

        protected override void OnDispose()
        {
            Items.CollectionChanged -= ItemsOnCollectionChanged;

            Mediator.Deregister(OnThemeChanged, ViewModelMessages.ThemeChanged);
        }
    }
}