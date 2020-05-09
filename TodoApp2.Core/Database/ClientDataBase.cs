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

        private const int NextDay = 10;

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
            string lastListOrder = m_DataAccess.GetTaskFirstListOrder();
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
                string lastListOrder = m_DataAccess.GetCategoryLastListOrder();
                categoryToAdd.ListOrder = GetNextListOrder(lastListOrder);

                // Persist category into database
                m_DataAccess.AddCategory(categoryToAdd);
                return true;
            }

            return false;
        }

        public void ReorderTask(TaskListItemViewModel task)
        {

        }

        //internal void UpdateTaskListOrders(ObservableCollection<TaskListItemViewModel> taskListItems)
        //{
        //    // Set the ListOrder property for each items
        //    SetItemsListOrders(taskListItems);

        //    // Persist the order changes into the database
        //    m_DataAccess.UpdateTaskListOrders(taskListItems);
        //}

        public void ReorderCategory(CategoryListItemViewModel category, int newPosition)
        {
            // Get all categories except the reordered one
            var categories = m_DataAccess.GetCategories().Where(c => c.Id != category.Id && !c.Trashed).ToList();

            // If the item moved to the end of the list,
            // the ListOrder is the last ListOrder + 30 minutes
            if (categories.Count == newPosition)
            {
                category.ListOrder = GetNextListOrder(categories[categories.Count - 1].ListOrder);
            }
            // If the item moved to the top of the list,
            // the ListOrder is the first ListOrder - 30 minutes
            else if (newPosition == 0)
            {
                category.ListOrder = GetPreviousListOrder(categories[0].ListOrder);
            }
            // Else the ListOrder should be in the middle of the previous and next order interval
            else
            {
                var previousListOrder = categories[newPosition - 1].ListOrder;
                var nextListOrder = categories[newPosition].ListOrder;

                DateTime.TryParse(previousListOrder, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime prev);
                DateTime.TryParse(nextListOrder, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime next);

                TimeSpan halfInterval = new TimeSpan((next - prev).Ticks / 2);

                DateTime newListOrderDateTime = prev.Add(halfInterval);
                category.ListOrder = newListOrderDateTime.ToString(DataAccessLayer.DateTimeFormat, CultureInfo.InvariantCulture);
            }

            m_DataAccess.UpdateCategory(category);
        }

        //internal void UpdateCategoryListOrders(ObservableCollection<CategoryListItemViewModel> categoryListItems)
        //{
        //    // Set the ListOrder property for each items
        //    SetItemsListOrders(categoryListItems);

        //    // Persist the order changes into the database
        //    m_DataAccess.UpdateCategoryListOrders(categoryListItems);
        //}

        /// <summary>
        /// Updates every Task item in the database
        /// </summary>
        /// <param name="taskList"></param>
        public void UpdateTaskList(ObservableCollection<TaskListItemViewModel> taskList)
        {
            // Set the ListOrder property for each items
            //ResetListOrders(taskList);

            // Persist every change in the list into the database
            m_DataAccess.UpdateTaskList(taskList);
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
            task.ListOrder = DateTime.MinValue.ToString(DataAccessLayer.DateTimeFormat, CultureInfo.InvariantCulture);

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
            category.ListOrder = DateTime.MinValue.ToString(DataAccessLayer.DateTimeFormat, CultureInfo.InvariantCulture);

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
            string lastListOrder = m_DataAccess.GetCategoryLastListOrder();
            category.ListOrder = GetNextListOrder(lastListOrder);

            // Persist the change into the database
            m_DataAccess.UpdateCategory(category);
        }

        /// <summary>
        /// Re-initializes the ListOrder property of each item in the list according to it's order in the list
        /// </summary>
        /// <param name="itemList"></param>
        private void ResetListOrders(IEnumerable<IReorderable> itemList)
        {
            DateTime current = DateTime.Now;

            // Update the ListOrder property of the IReorderable with DateTimes.
            // The next DateTime is half an hour apart from the previous one
            foreach (var item in itemList)
            {
                item.ListOrder = current.ToString(DataAccessLayer.DateTimeFormat, CultureInfo.InvariantCulture);
                current = current.AddDays(NextDay);
            }
        }

        /// <summary>
        /// Gets the next available ListOrder relative to the provided one
        /// </summary>
        /// <param name="currentListOrder"></param>
        /// <returns></returns>
        private string GetNextListOrder(string currentListOrder)
        {
            DateTime.TryParse(currentListOrder, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime currentListOrderDateTime);
            DateTime nextListOrder = currentListOrderDateTime.AddDays(NextDay);
            return nextListOrder.ToString(DataAccessLayer.DateTimeFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the previous available ListOrder relative to the provided one
        /// </summary>
        /// <param name="currentListOrder"></param>
        /// <returns></returns>
        private string GetPreviousListOrder(string currentListOrder)
        {
            DateTime.TryParse(currentListOrder, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime currentListOrderDateTime);
            DateTime previousListOrder = currentListOrderDateTime.AddDays(-NextDay);
            return previousListOrder.ToString(DataAccessLayer.DateTimeFormat, CultureInfo.InvariantCulture);
        }

        public void Dispose()
        {
            m_DataAccess.Dispose();
        }
    }
}
