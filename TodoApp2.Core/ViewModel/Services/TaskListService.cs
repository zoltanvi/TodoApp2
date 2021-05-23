using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    /// <summary>
    /// Service to hold the task item list.
    /// Because this is a service, it can be accessed from multiple ViewModels.
    /// </summary>
    public class TaskListService : BaseViewModel
    {
        private readonly CategoryListService m_CategoryListService;
        private readonly IDatabase m_Database;
        private string CurrentCategory => m_CategoryListService.CurrentCategory;

        /// <summary>
        /// The task list items
        /// </summary>
        public ObservableCollection<TaskListItemViewModel> TaskPageItems { get; }
        
        public TaskListService(IDatabase database, CategoryListService categoryListService)
        {
            m_Database = database;
            m_CategoryListService = categoryListService;

            // Query the items with the current category
            List<TaskListItemViewModel> items = m_Database.GetActiveTaskItems(CurrentCategory);

            // Fill the actual list with the queried items
            TaskPageItems = new ObservableCollection<TaskListItemViewModel>(items);

            m_Database.TaskChanged += OnClientDatabaseTaskChanged;

            // Subscribe to the category changed event to filter the list when it happens
            Mediator.Register(OnCategoryChanged, ViewModelMessages.CategoryChanged);
        }

        public void AddNewTask(TaskListItemViewModel task)
        {
            // Persist into database and set the task ID
            // Note: The database gives the ID to the task
            m_Database.AddTask(task);

            // Add the task into the list
            TaskPageItems.Insert(0, task);
        }

        public void UpdateTask(TaskListItemViewModel task)
        {
            TaskListItemViewModel taskToUpdate = TaskPageItems.FirstOrDefault(item => item.Id == task.Id);
            taskToUpdate?.CopyProperties(task);
            m_Database.UpdateTask(task);
        }

        /// <inheritdoc cref="Core.Database.ReorderTask"/>
        /// <remarks>Updates the task in the list also.</remarks>
        public void ReorderTask(TaskListItemViewModel task, int newPosition)
        {
            TaskListItemViewModel taskToUpdate = TaskPageItems.FirstOrDefault(item => item.Id == task.Id);
            taskToUpdate?.CopyProperties(task);
            m_Database.ReorderTask(task, newPosition);
        }

        public void RemoveTask(TaskListItemViewModel task)
        {
            TaskPageItems.Remove(task);
        }

        public void PersistTaskList()
        {
            m_Database.UpdateTaskList(TaskPageItems);
        }

        /// <inheritdoc cref="Core.Database.GetActiveTaskItemsAsync"/>
        public async Task<List<TaskListItemViewModel>> GetActiveTaskItemsAsync(string categoryName)
        {
            return await m_Database.GetActiveTaskItemsAsync(categoryName);
        }

        private void OnClientDatabaseTaskChanged(object sender, TaskChangedEventArgs e)
        {
            TaskListItemViewModel modifiedItem = TaskPageItems.FirstOrDefault(item => item.Id == e.Task.Id);

            modifiedItem?.CopyProperties(e.Task);
        }

        private async Task OnCategoryChanged()
        {
            // Clear the list first to prevent inconsistent data on UI while the items are loading
            TaskPageItems.Clear();

            // Query the items with the current category
            List<TaskListItemViewModel> filteredItems = await GetActiveTaskItemsAsync(CurrentCategory);

            // Clear the list to prevent showing items from multiple categories.
            // This can happen if the user changes category again while the query runs
            TaskPageItems.Clear();

            // Fill the actual list with the queried items
            TaskPageItems.AddRange(filteredItems);
        }
    }
}
