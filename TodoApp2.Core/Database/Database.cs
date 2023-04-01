using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    /// <summary>
    /// Represents the application client database
    /// </summary>
    public class Database : IDatabase, IDisposable
    {
        private DataAccessLayer m_DataAccess;
        private Reorderer m_Reorderer;

        public event EventHandler<TaskChangedEventArgs> TaskChanged;
        public event EventHandler<CategoryChangedEventArgs> CategoryChanged;

        public Database()
        {
            m_Reorderer = new Reorderer();
            m_DataAccess = new DataAccessLayer();

            // Initialize the database
            m_DataAccess.InitializeDatabase();
        }

        public async Task<List<TaskViewModel>> GetActiveTasksAsync(CategoryViewModel category)
        {
            List<TaskViewModel> returnValue = await Task.Run(() => GetActiveTasks(category));
            return returnValue;
        }

        public List<TaskViewModel> GetActiveTasks(CategoryViewModel category)
        {
            if (string.IsNullOrEmpty(category?.Name))
            {
                return new List<TaskViewModel>();
            }

            // Returns the task list from the database ordered by ListOrder column
            List<TaskViewModel> tasksFromCategory = m_DataAccess.TaskDataAccess.GetActiveTasks(category.Id);

            return tasksFromCategory;
        }

        public List<TaskViewModel> GetActiveTasksWithReminder() => m_DataAccess.TaskDataAccess.GetActiveTasksWithReminder();

        public TaskViewModel GetTask(int id) => m_DataAccess.TaskDataAccess.GetTask(id);

        public CategoryViewModel GetCategory(string categoryName) => m_DataAccess.CategoryDataAccess.GetCategory(categoryName);

        public CategoryViewModel GetCategory(int categoryId) => m_DataAccess.CategoryDataAccess.GetCategory(categoryId);

        public List<CategoryViewModel> GetValidCategories() => m_DataAccess.CategoryDataAccess.GetActiveCategories();

        public List<Setting> GetSettings() => m_DataAccess.SettingsDataAccess.GetSettings();

        public void AddSetting(Setting setting)
        {
            // Generate an ID for the item
            setting.Id = m_DataAccess.SettingsDataAccess.GetNextId();

            // Persist record into database
            m_DataAccess.SettingsDataAccess.AddSetting(setting);
        }

        /// <summary>
        /// Updates all settings entry in the database if exists,
        /// adds the entry if it not exists.
        /// </summary>
        /// <param name="settings"></param>
        public void UpdateSettings(List<Setting> settings)
        {
            List<Setting> existingSettings = m_DataAccess.SettingsDataAccess.GetSettings();

            Dictionary<string, Setting> existingSettingsDictionary =
                existingSettings.ToDictionary(settingsModel => settingsModel.Key);

            IEnumerable<Setting> settingsToUpdate = settings.Where(s => existingSettingsDictionary.ContainsKey(s.Key));
            IEnumerable<Setting> settingsToAdd = settings.Where(s => !existingSettingsDictionary.ContainsKey(s.Key));

            if (settingsToAdd.Any())
            {
                int nextId = m_DataAccess.SettingsDataAccess.GetNextId();

                foreach (var settingsModel in settingsToAdd)
                {
                    settingsModel.Id = nextId++;
                }

                m_DataAccess.SettingsDataAccess.AddSettings(settingsToAdd);
            }

            m_DataAccess.SettingsDataAccess.UpdateSettings(settingsToUpdate);
        }

        public TaskViewModel CreateTask(string taskContent, int categoryId, int position)
        {
            TaskViewModel createdTask = m_DataAccess.TaskDataAccess.CreateTask(taskContent, categoryId);

            ReorderTask(createdTask, position);

            return createdTask;
        }

        public void UpdateTask(TaskViewModel task)
        {
            m_DataAccess.TaskDataAccess.UpdateTask(task);

            TaskChanged?.Invoke(this, new TaskChangedEventArgs(task));
        }

        public bool AddCategoryIfNotExists(CategoryViewModel categoryToAdd)
        {
            List<CategoryViewModel> categoryList = m_DataAccess.CategoryDataAccess.GetCategories();

            // If the category exists in the database, do nothing
            if (categoryList.All(category => category.Name != categoryToAdd.Name))
            {
                // Generate an ID for the item
                categoryToAdd.Id = m_DataAccess.CategoryDataAccess.GetNextId();

                // Generate a ListOrder for the item
                long lastListOrder = m_DataAccess.CategoryDataAccess.GetLastListOrder();
                categoryToAdd.ListOrder = m_Reorderer.GetNextListOrder(lastListOrder);

                // Persist category into database
                m_DataAccess.CategoryDataAccess.AddCategory(categoryToAdd);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Modifies the task order in the list to the provided <see cref="newPosition"/>.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="newPosition"></param>
        public void ReorderTask(TaskViewModel task, int newPosition)
        {
            // Get all non-trashed task items from the task's category
            List<TaskViewModel> activeTasks =
                m_DataAccess.TaskDataAccess.GetActiveTasks(task.CategoryId);

            // Filter out the reordered task
            List<IReorderable> filteredOrderedTasks =
                activeTasks.Where(t => t.Id != task.Id).Cast<IReorderable>().ToList();

            IReorderable itemToReorder = task as IReorderable;

            m_Reorderer.ReorderItem(filteredOrderedTasks, itemToReorder, newPosition, UpdateTaskListOrder);

            m_DataAccess.TaskDataAccess.UpdateTask(task);

            TaskChanged?.Invoke(this, new TaskChangedEventArgs(task));
        }

        /// <summary>
        /// Modifies the category order in the list to the provided <see cref="newPosition"/>.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="newPosition"></param>
        public void ReorderCategory(CategoryViewModel category, int newPosition)
        {
            List<CategoryViewModel> activeCategories =
                m_DataAccess.CategoryDataAccess.GetActiveCategories();

            // Get all categories except the reordered one that are not trashed
            List<IReorderable> orderedCategories =
                activeCategories.Where(c => c.Id != category.Id).Cast<IReorderable>().ToList();

            IReorderable itemToReorder = category as IReorderable;

            m_Reorderer.ReorderItem(
                orderedCategories,
                itemToReorder,
                newPosition,
                UpdateCategoryListOrder);

            m_DataAccess.CategoryDataAccess.UpdateCategory(category);

            // Category changed event?
        }

        public void UpdateCategoryListOrder(IEnumerable<IReorderable> categoryList)
        {
            IEnumerable<CategoryViewModel> updateSource = categoryList.Cast<CategoryViewModel>();

            // Persist every ORDER change in the list into the database
            m_DataAccess.CategoryDataAccess.UpdateCategoryListOrders(updateSource);
        }

        public void UpdateTaskListOrder(IEnumerable<IReorderable> taskList) =>
            m_DataAccess.TaskDataAccess.UpdateTaskListOrders(taskList.Cast<TaskViewModel>());

        public void UpdateTasks(IEnumerable<IReorderable> taskList) =>
            m_DataAccess.TaskDataAccess.UpdateTaskList(taskList.Cast<TaskViewModel>());

        public void TrashCategory(CategoryViewModel category)
        {
            category.Trashed = true;
            category.ListOrder = long.MinValue;

            m_DataAccess.CategoryDataAccess.UpdateCategory(category);
        }

        public void UntrashCategory(CategoryViewModel category)
        {
            category.Trashed = false;

            // Set the order to the end of the list
            long lastListOrder = m_DataAccess.CategoryDataAccess.GetLastListOrder();
            category.ListOrder = m_Reorderer.GetNextListOrder(lastListOrder);

            m_DataAccess.CategoryDataAccess.UpdateCategory(category);
        }

        public void UpdateCategory(CategoryViewModel category)
        {
            CategoryViewModel originalCategory = m_DataAccess.CategoryDataAccess.GetCategory(category.Id);

            m_DataAccess.CategoryDataAccess.UpdateCategory(category);

            CategoryChanged?.Invoke(this, new CategoryChangedEventArgs(originalCategory, category));
        }

        public NoteViewModel CreateNote(string noteTitle)
        {
            NoteViewModel createdNote = m_DataAccess.NoteDataAccess.CreateNote(noteTitle);
            return createdNote;
        }
        public List<NoteViewModel> GetValidNotes() => m_DataAccess.NoteDataAccess.GetActiveNotes();

        public NoteViewModel GetNote(int noteId) => m_DataAccess.NoteDataAccess.GetNote(noteId);

        public void UpdateNote(NoteViewModel note)
        {
            if (note != null)
            {
                m_DataAccess.NoteDataAccess.UpdateNote(note);
            }
        }


        /// <summary>
        /// Modifies the note order in the list to the provided <see cref="newPosition"/>.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="newPosition"></param>
        public void ReorderNote(NoteViewModel note, int newPosition)
        {
            // TODO: make it generic!
            List<NoteViewModel> activeNotes =
                m_DataAccess.NoteDataAccess.GetActiveNotes();

            // Get all categories except the reordered one that are not trashed
            List<IReorderable> orderedNotes =
                activeNotes.Where(c => c.Id != note.Id).Cast<IReorderable>().ToList();

            IReorderable itemToReorder = note as IReorderable;

            m_Reorderer.ReorderItem(
                orderedNotes,
                itemToReorder,
                newPosition,
                UpdateCategoryListOrder);

            m_DataAccess.NoteDataAccess.UpdateNote(note);

            // Category changed event?
        }

        public void Dispose()
        {
            m_DataAccess?.Dispose();
        }
    }
}