using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
            if (string.IsNullOrEmpty(categoryName))
            {
                return new List<TaskListItemViewModel>();
            }

            // Get category with the specified name
            CategoryListItemViewModel category = m_DataAccess.GetCategory(categoryName);

            // Returns the task list from the database ordered by ListOrder column
            List<TaskListItemViewModel> allTasks = m_DataAccess.GetTasks(category.Id);

            // Return only the items that are not trashed
            return allTasks.Where(task => task.Trashed == false).ToList();
        }

        internal CategoryListItemViewModel GetCategory(string currentCategory)
        {
            return m_DataAccess.GetCategory(currentCategory);
        }
        
        public List<CategoryListItemViewModel> GetActiveCategories()
        {
            // Get all categories
            List<CategoryListItemViewModel> allCategories = m_DataAccess.GetCategories();

            // Return only the categories that are not trashed
            return allCategories.Where(category => category.Trashed == false).ToList();
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

        internal void UpdateCategoryListOrders(ObservableCollection<CategoryListItemViewModel> categoryListItems)
        {
            // Set the ListOrder property for each items
            SetItemsListOrders(categoryListItems);

            // Persist the order changes into the database
            m_DataAccess.UpdateCategoryListOrders(categoryListItems);
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

        internal void TrashCategory(CategoryListItemViewModel category)
        {
            // Set Trashed property to true so it won't be listed in the active list
            category.Trashed = true;
            
            // Indicate that it is an invalid order
            category.ListOrder = -1;
           
            // Persist the change into the database
            m_DataAccess.UpdateCategory(category);
        }

        internal CategoryListItemViewModel UntrashCategoryIfExists(CategoryListItemViewModel categoryToUntrash)
        {
            CategoryListItemViewModel category = m_DataAccess.GetCategory(categoryToUntrash.Name);
            
            // If the category exits in the database, set trashed to false and persist the change
            if (category != null)
            {
                category.ListOrder = categoryToUntrash.ListOrder;
                category.Trashed = false;

                m_DataAccess.UpdateCategory(category);
            }

            return category;
        }

        private void SetItemsListOrders(ObservableCollection<TaskListItemViewModel> taskListItems)
        {
            // Update the ListOrder property of the TaskItems with their order in the provided list
            for (int i = 0; i < taskListItems.Count; i++)
            {
                taskListItems[i].ListOrder = i;
            }
        }

        private void SetItemsListOrders(ObservableCollection<CategoryListItemViewModel> categoryListItems)
        {
            // Update the ListOrder property of the TaskItems with their order in the provided list
            for (int i = 0; i < categoryListItems.Count; i++)
            {
                categoryListItems[i].ListOrder = i;
            }
        }

        public void Dispose()
        {
            m_DataAccess.Dispose();
        }
    }
}
