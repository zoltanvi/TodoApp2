using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ninject.Infrastructure.Disposal;

namespace TodoApp2.Core
{
    /// <summary>
    /// Represents the application client database
    /// </summary>
    public class ClientDatabase : IDisposable
    {
        private readonly DataAccessLayer m_DataAccess;

        public ClientDatabase()
        {
            m_DataAccess = new DataAccessLayer();

            // Initialize the database
            m_DataAccess.InitializeDatabase();
        }

        public List<TaskListItemViewModel> GetActiveTaskItems(string categoryName)
        {
            // Returns the task list from the database ordered by ListOrder column
            List<TaskListItemViewModel> allTasks = m_DataAccess.GetTasks(categoryName);

            // Returns only the items that are not trashed
            return allTasks.Where(task => task.Trashed == false).ToList();
        }

        internal List<CategoryListItemViewModel> GetCategories()
        {
            return m_DataAccess.GetCategories();
        }

        public void AddTask(TaskListItemViewModel taskListItem)
        {
            // Add task to database, get back the generated ID
            int taskId = m_DataAccess.AddTask(taskListItem);

            // Set the generated ID to the task in the memory
            taskListItem.Id = taskId;
        }

        internal bool AddCategory(CategoryListItemViewModel categoryToAdd)
        {
            return m_DataAccess.AddCategoryIfNotExists(categoryToAdd);   
        }

        internal void UpdateTaskListOrders(ObservableCollection<TaskListItemViewModel> taskListItems)
        {
            // Set the ListOrder property for each items
            SetItemsListOrders(taskListItems);

            // Persist the order changes into the database
            m_DataAccess.UpdateTaskListOrders(taskListItems);
        }
        
        internal void UpdateTaskList(ObservableCollection<TaskListItemViewModel> taskListItems)
        {
            // Set the ListOrder property for each items
            SetItemsListOrders(taskListItems);

            // Persist every change in the list into the database
            m_DataAccess.UpdateTaskList(taskListItems);
        }


        internal void TrashTask(TaskListItemViewModel task)
        {
            // Set Trashed property to true so it won't be listed in the active list
            task.Trashed = true;

            // Indicate that it is an invalid order
            task.ListOrder = -1;

            // Persist the change into the database
            m_DataAccess.UpdateTask(task);
        }

        internal void DeleteCategory(CategoryListItemViewModel category)
        {
            m_DataAccess.DeleteCategory(category);
        }

        private void SetItemsListOrders(ObservableCollection<TaskListItemViewModel> taskListItems)
        {
            // Update the ListOrder property of the TaskItems with their order in the provided list
            for (int i = 0; i < taskListItems.Count; i++)
            {
                taskListItems[i].ListOrder = i;
            }
        }

        public void Dispose()
        {
            m_DataAccess.Dispose();
        }
    }
}
