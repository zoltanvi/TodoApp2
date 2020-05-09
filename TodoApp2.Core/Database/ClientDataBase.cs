using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

        /// <summary>
        /// Gets the task items from the provided category which are not trashed
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<TaskListItemViewModel> GetActiveTaskItems(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return new List<TaskListItemViewModel>();
            }

            // Get category with the specified name
            CategoryListItemViewModel category = m_DataAccess.GetCategory(categoryName);

            // Returns the task list from the database ordered by ListOrder column
            List<TaskListItemViewModel> allTasks = m_DataAccess.GetTasks();

            // Return only the items from the provided category which are not trashed
            return allTasks.Where(task => task.CategoryId == category.Id && task.Trashed == false).ToList();
        }

        /// <summary>
        /// Gets the category item with the provided name
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public CategoryListItemViewModel GetCategory(string categoryName)
        {
            return m_DataAccess.GetCategory(categoryName);
        }

        /// <summary>
        /// Gets the category items which are not trashed
        /// </summary>
        /// <returns></returns>
        public List<CategoryListItemViewModel> GetActiveCategories()
        {
            // Get all categories
            List<CategoryListItemViewModel> allCategories = m_DataAccess.GetCategories();

            // Return only the categories that are not trashed
            return allCategories.Where(category => category.Trashed == false).ToList();
        }

        /// <summary>
        /// Inserts a task into the database
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(TaskListItemViewModel task)
        {
            // Generate an ID for the item
            task.Id = m_DataAccess.GetTaskNextId();

            // Generate a ListOrder for the item
            long lastListOrder = m_DataAccess.GetTaskFirstListOrder();
            task.ListOrder = GetPreviousListOrder(lastListOrder);

            // Persist task into database
            m_DataAccess.AddTask(task);
        }

        /// <summary>
        /// Inserts a category into the database if it not exists
        /// </summary>
        /// <param name="categoryToAdd"></param>
        /// <returns>Returns true, if the category is added, false otherwise</returns>
        public bool AddCategoryIfNotExists(CategoryListItemViewModel categoryToAdd)
        {
            List<CategoryListItemViewModel> categoryList = m_DataAccess.GetCategories();

            // If the category exists in the database, do nothing 
            if (categoryList.All(category => category.Name != categoryToAdd.Name))
            {
                // Generate an ID for the item
                categoryToAdd.Id = m_DataAccess.GetCategoryNextId();

                // Generate a ListOrder for the item
                long lastListOrder = m_DataAccess.GetCategoryLastListOrder();
                categoryToAdd.ListOrder = GetNextListOrder(lastListOrder);

                // Persist category into database
                m_DataAccess.AddCategory(categoryToAdd);
                return true;
            }

            return false;
        }

        public void ReorderTasks(TaskListItemViewModel task, int newPosition)
        {
            // Get all tasks except the reordered one
            var orderedTasks = m_DataAccess.GetTasks().Where(t => t.Id != task.Id && !t.Trashed)
                .Cast<IReorderable>().ToList();
            var itemToReorder = task as IReorderable;
            ReorderItem(orderedTasks, itemToReorder, newPosition, UpdateTaskListOrder);
            m_DataAccess.UpdateTask(task);
        }

        public void ReorderCategory(CategoryListItemViewModel category, int newPosition)
        {
            // Get all categories except the reordered one
            var orderedCategories = m_DataAccess.GetCategories().Where(c => c.Id != category.Id && !c.Trashed)
                .Cast<IReorderable>().ToList();
            var itemToReorder = category as IReorderable;
            ReorderItem(orderedCategories, itemToReorder, newPosition, UpdateCategoryListOrder);
            m_DataAccess.UpdateCategory(category);
        }

        /// <summary>
        /// Updates every Category item order in the database
        /// </summary>
        /// <param name="categoryList"></param>
        public void UpdateCategoryListOrder(IEnumerable<IReorderable> categoryList)
        {
            var updateSource = categoryList.Cast<CategoryListItemViewModel>();
            // Persist every order change in the list into the database
            m_DataAccess.UpdateCategoryListOrders(updateSource);
        }

        /// <summary>
        /// Updates every Task item order in the database
        /// </summary>
        /// <param name="taskList"></param>
        public void UpdateTaskListOrder(IEnumerable<IReorderable> taskList)
        {
            var updateSource = taskList.Cast<TaskListItemViewModel>();
            // Persist every order change in the list into the database
            m_DataAccess.UpdateTaskListOrders(updateSource);
        }


        /// <summary>
        /// Updates every Task item in the database
        /// </summary>
        /// <param name="taskList"></param>
        public void UpdateTaskList(IEnumerable<IReorderable> taskList)
        {
            var updateSource = taskList.Cast<TaskListItemViewModel>();
            // Persist every change in the list into the database
            m_DataAccess.UpdateTaskList(updateSource);
        }

        /// <summary>
        /// Trashes the task
        /// </summary>
        /// <param name="task"></param>
        public void TrashTask(TaskListItemViewModel task)
        {
            // Set Trashed property to true so it won't be listed in the active list
            task.Trashed = true;

            // Indicate that it is an invalid order
            task.ListOrder = long.MinValue;

            // Persist the change into the database
            m_DataAccess.UpdateTask(task);
        }

        /// <summary>
        /// Trashes the category
        /// </summary>
        /// <param name="category"></param>
        public void TrashCategory(CategoryListItemViewModel category)
        {
            // Set Trashed property to true so it won't be listed in the active list
            category.Trashed = true;

            // Indicate that it is an invalid order
            category.ListOrder = long.MinValue;

            // Persist the change into the database
            m_DataAccess.UpdateCategory(category);
        }

        /// <summary>
        /// Sets the Trashed property of the category to false
        /// </summary>
        /// <param name="category"></param>
        public void UntrashCategory(CategoryListItemViewModel category)
        {
            // Set Trashed property to true so it won't be listed in the active list
            category.Trashed = false;

            // Set the order to the end of the list
            long lastListOrder = m_DataAccess.GetCategoryLastListOrder();
            category.ListOrder = GetNextListOrder(lastListOrder);

            // Persist the change into the database
            m_DataAccess.UpdateCategory(category);
        }


        private void ReorderItem(List<IReorderable> orderedItems, IReorderable itemToReorder,
            int newPosition, Action<IEnumerable<IReorderable>> updateStrategy)
        {
            // If the item moved to the end of the list, calculate the next order for it
            if (orderedItems.Count == newPosition)
            {
                itemToReorder.ListOrder = GetNextListOrder(orderedItems[orderedItems.Count - 1].ListOrder);
            }
            // If the item moved to the top of the list, calculate the previous order for it
            else if (newPosition == 0)
            {
                itemToReorder.ListOrder = GetPreviousListOrder(orderedItems[0].ListOrder);
            }
            // Else the ListOrder should be in the middle between the previous and next order interval
            else
            {
                long previousListOrder = orderedItems[newPosition - 1].ListOrder;
                long nextListOrder = orderedItems[newPosition].ListOrder;
                long newListOrder = previousListOrder + ((nextListOrder - previousListOrder) / 2);

                itemToReorder.ListOrder = newListOrder;

                // If there is no room between 2 existing order, reorder the whole list
                if (newListOrder == previousListOrder || newListOrder == nextListOrder)
                {
                    orderedItems.Insert(newPosition, itemToReorder);
                    ResetListOrders(orderedItems);
                    updateStrategy(orderedItems);
                }
            }
        }

        /// <summary>
        /// Re-initializes the ListOrder property of each item in the list according to it's order in the list
        /// </summary>
        /// <param name="itemList"></param>
        private void ResetListOrders(IEnumerable<IReorderable> itemList)
        {
            long current = DataAccessLayer.DefaultListOrder;

            // Update the ListOrder property of the IReorderable items
            foreach (var item in itemList)
            {
                item.ListOrder = current;
                current += DataAccessLayer.ListOrderInterval;
            }
        }


        /// <summary>
        /// Gets the next available ListOrder relative to the provided one
        /// </summary>
        /// <param name="currentListOrder"></param>
        /// <returns></returns>
        private long GetNextListOrder(long currentListOrder)
        {
            return currentListOrder + DataAccessLayer.ListOrderInterval;
        }

        /// <summary>
        /// Gets the previous available ListOrder relative to the provided one
        /// </summary>
        /// <param name="currentListOrder"></param>
        /// <returns></returns>
        private long GetPreviousListOrder(long currentListOrder)
        {
            return currentListOrder - DataAccessLayer.ListOrderInterval;
        }

        public void Dispose()
        {
            m_DataAccess.Dispose();
        }
    }
}
