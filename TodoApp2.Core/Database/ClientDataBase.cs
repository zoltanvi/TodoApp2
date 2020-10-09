using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<List<TaskListItemViewModel>> GetActiveTaskItemsAsync(string categoryName)
        {
            var returnValue = await Task.Run(() => GetActiveTaskItems(categoryName));
            return returnValue;
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
        /// Gets all task items which are not trashed
        /// </summary>
        /// <returns></returns>
        public List<TaskListItemViewModel> GetActiveTaskItems()
        {
            // Returns the task list from the database ordered by ListOrder column
            List<TaskListItemViewModel> allTasks = m_DataAccess.GetTasks();

            // Return only the items which are not trashed
            return allTasks.Where(task => task.Trashed == false).ToList();
        }

        /// <summary>
        /// Gets the task with the provided ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TaskListItemViewModel GetTask(int id)
        {
            return m_DataAccess.GetTask(id);
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
        /// Gets the category item with the provided ID
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public CategoryListItemViewModel GetCategory(int categoryId)
        {
            // TODO: write query for it
            var allCategories = m_DataAccess.GetCategories();
            return allCategories.FirstOrDefault(category => category.Id == categoryId);
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
        /// Gets every settings entry from the database
        /// </summary>
        /// <returns></returns>
        public List<SettingsModel> GetSettings()
        {
            return m_DataAccess.GetSettings();
        }

        /// <summary>
        /// Inserts a setting into the database
        /// </summary>
        /// <param name="setting"></param>
        public void AddSetting(SettingsModel setting)
        {
            // Generate an ID for the item
            setting.Id = m_DataAccess.GetSettingsNextId();

            // Persist record into database
            m_DataAccess.AddSetting(setting);
        }

        /// <summary>
        /// Updates all settings entry in the database if exists,
        /// adds the entry if it not exists.
        /// </summary>
        /// <param name="settings"></param>
        internal void UpdateSettings(List<SettingsModel> settings)
        {
            List<SettingsModel> existingSettings = m_DataAccess.GetSettings();

            var existingSettingsDictionary = existingSettings.ToDictionary(settingsModel => settingsModel.Key);

            var settingsToUpdate = settings.Where(s => existingSettingsDictionary.ContainsKey(s.Key));
            var settingsToAdd = settings.Where(s => !existingSettingsDictionary.ContainsKey(s.Key));

            int nextId = m_DataAccess.GetSettingsNextId();

            foreach (var settingsModel in settingsToAdd)
            {
                settingsModel.Id = nextId++;
            }

            m_DataAccess.AddSettings(settingsToAdd);

            m_DataAccess.UpdateSettings(settingsToUpdate);
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
        /// Updates a task in the database
        /// </summary>
        /// <param name="task"></param>
        public void UpdateTask(TaskListItemViewModel task)
        {
            m_DataAccess.UpdateTask(task);
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

        /// <summary>
        /// Modifies the task order in the list to the provided <see cref="newPosition"/>.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="newPosition"></param>
        public void ReorderTask(TaskListItemViewModel task, int newPosition)
        {
            // Get all tasks except the reordered one
            var orderedTasks = m_DataAccess.GetTasks().Where(t => t.Id != task.Id && !t.Trashed)
                .Cast<IReorderable>().ToList();
            var itemToReorder = task as IReorderable;
            ReorderItem(orderedTasks, itemToReorder, newPosition, UpdateTaskListOrder);
            m_DataAccess.UpdateTask(task);
        }

        /// <summary>
        /// Modifies the category order in the list to the provided <see cref="newPosition"/>.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="newPosition"></param>
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

        /// <summary>
        /// Reorders the given item in it's containing collection.
        /// </summary>
        /// <remarks>
        /// The order for every element in the collection may will be overwritten!
        /// The algorithm only modifies a single item until there is room between two item order.
        /// When there is no more room between two item order, every item in the collection gets a
        /// new number as it's order.
        /// </remarks>
        /// <param name="orderedItems">The collection without the item to reorder.</param>
        /// <param name="itemToReorder">The item to reorder.</param>
        /// <param name="newPosition">The item's new position in the collection.</param>
        /// <param name="updateStrategy">The action that is called when each element
        /// in the collection got a new order number. This action should persist the changes.</param>
        private void ReorderItem(List<IReorderable> orderedItems, IReorderable itemToReorder,
            int newPosition, Action<IEnumerable<IReorderable>> updateStrategy)
        {
            // If there is no other item besides the itemToReorder, there is nothing to do
            if (orderedItems.Count == 0) return;

            // If the item moved to the top of the list, calculate the previous order for it
            if (newPosition == 0)
            {
                itemToReorder.ListOrder = GetPreviousListOrder(orderedItems[0].ListOrder);
            }
            // If the item moved to the end of the list, calculate the next order for it
            else if (orderedItems.Count == newPosition)
            {
                itemToReorder.ListOrder = GetNextListOrder(orderedItems[orderedItems.Count - 1].ListOrder);
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