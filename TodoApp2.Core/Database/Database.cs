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
        private DataAccessLayer _dataAccess;
        private Reorderer _reorderer;

        public event EventHandler<TaskChangedEventArgs> TaskChanged;
        public event EventHandler<CategoryChangedEventArgs> CategoryChanged;

        public Database()
        {
            _reorderer = new Reorderer();
            _dataAccess = new DataAccessLayer();

            // Initialize the database
            _dataAccess.InitializeDatabase();
        }

        public void AddDefaultRecords()
        {
            _dataAccess.AddDefaultRecords();
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
            List<TaskViewModel> tasksFromCategory = _dataAccess.TaskDataAccess.GetActiveTasks(category.Id);

            return tasksFromCategory;
        }

        public List<TaskViewModel> GetActiveTasksWithReminder() => _dataAccess.TaskDataAccess.GetActiveTasksWithReminder();

        public TaskViewModel GetTask(int id) => _dataAccess.TaskDataAccess.GetTask(id);

        public CategoryViewModel GetCategory(string categoryName) => _dataAccess.CategoryDataAccess.GetCategory(categoryName);

        public CategoryViewModel GetCategory(int categoryId) => _dataAccess.CategoryDataAccess.GetCategory(categoryId);

        public List<CategoryViewModel> GetValidCategories() => _dataAccess.CategoryDataAccess.GetActiveCategories();

        public List<Setting> GetSettings() => _dataAccess.SettingsDataAccess.GetSettings();

        /// <summary>
        /// Updates all settings entry in the database if exists,
        /// adds the entry if it not exists.
        /// </summary>
        /// <param name="settings"></param>
        public void UpdateSettings(List<Setting> settings)
        {
            List<Setting> existingSettings = _dataAccess.SettingsDataAccess.GetSettings();

            Dictionary<string, Setting> existingSettingsMap = 
                existingSettings.ToDictionary(settingsModel => settingsModel.Key);

            IEnumerable<Setting> settingsToUpdate = settings
                .Where(s => existingSettingsMap.ContainsKey(s.Key))
                .Where(s => s.Value != existingSettingsMap[s.Key].Value);

            IEnumerable<Setting> settingsToAdd = settings
                .Where(s => !existingSettingsMap.ContainsKey(s.Key));

            if (settingsToAdd.Any())
            {
                _dataAccess.SettingsDataAccess.AddSettings(settingsToAdd);
            }

            if (settingsToUpdate.Any())
            {
                _dataAccess.SettingsDataAccess.UpdateSettings(settingsToUpdate);
            }
        }

        public TaskViewModel CreateTask(string taskContent, int categoryId, int position)
        {
            TaskViewModel createdTask = _dataAccess.TaskDataAccess.CreateTask(taskContent, categoryId);

            ReorderTask(createdTask, position);

            return createdTask;
        }

        public void UpdateTask(TaskViewModel task)
        {
            _dataAccess.TaskDataAccess.UpdateTask(task);

            TaskChanged?.Invoke(this, new TaskChangedEventArgs(task));
        }

        public bool AddCategoryIfNotExists(CategoryViewModel categoryToAdd)
        {
            List<CategoryViewModel> categoryList = _dataAccess.CategoryDataAccess.GetCategories();

            // If the category exists in the database, do nothing
            if (categoryList.All(category => category.Name != categoryToAdd.Name))
            {
                // Generate an ID for the item
                categoryToAdd.Id = _dataAccess.CategoryDataAccess.GetNextId();

                // Generate a ListOrder for the item
                long lastListOrder = _dataAccess.CategoryDataAccess.GetLastListOrder();
                categoryToAdd.ListOrder = _reorderer.GetNextListOrder(lastListOrder);

                // Persist category into database
                _dataAccess.CategoryDataAccess.AddCategory(categoryToAdd);
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
                _dataAccess.TaskDataAccess.GetActiveTasks(task.CategoryId);

            // Filter out the reordered task
            List<IReorderable> filteredOrderedTasks =
                activeTasks.Where(t => t.Id != task.Id).Cast<IReorderable>().ToList();

            IReorderable itemToReorder = task as IReorderable;

            _reorderer.ReorderItem(filteredOrderedTasks, itemToReorder, newPosition, UpdateTaskListOrder);

            _dataAccess.TaskDataAccess.UpdateTask(task);

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
                _dataAccess.CategoryDataAccess.GetActiveCategories();

            // Get all categories except the reordered one that are not trashed
            List<IReorderable> orderedCategories =
                activeCategories.Where(c => c.Id != category.Id).Cast<IReorderable>().ToList();

            IReorderable itemToReorder = category as IReorderable;

            _reorderer.ReorderItem(
                orderedCategories,
                itemToReorder,
                newPosition,
                UpdateCategoryListOrder);

            _dataAccess.CategoryDataAccess.UpdateCategory(category);
        }

        public void UpdateCategoryListOrder(IEnumerable<IReorderable> categoryList)
        {
            IEnumerable<CategoryViewModel> updateSource = categoryList.Cast<CategoryViewModel>();

            // Persist every ORDER change in the list into the database
            _dataAccess.CategoryDataAccess.UpdateCategoryListOrders(updateSource);
        }

        public void UpdateTaskListOrder(IEnumerable<IReorderable> taskList) =>
            _dataAccess.TaskDataAccess.UpdateTaskListOrders(taskList.Cast<TaskViewModel>());

        public void UpdateTasks(IEnumerable<IReorderable> taskList) =>
            _dataAccess.TaskDataAccess.UpdateTaskList(taskList.Cast<TaskViewModel>());

        public void TrashCategory(CategoryViewModel category)
        {
            category.Trashed = true;
            category.ListOrder = long.MinValue;

            _dataAccess.CategoryDataAccess.UpdateCategory(category);
        }

        public void UntrashCategory(CategoryViewModel category)
        {
            category.Trashed = false;

            // Set the order to the end of the list
            long lastListOrder = _dataAccess.CategoryDataAccess.GetLastListOrder();
            category.ListOrder = _reorderer.GetNextListOrder(lastListOrder);

            _dataAccess.CategoryDataAccess.UpdateCategory(category);
        }

        public void UpdateCategory(CategoryViewModel category)
        {
            CategoryViewModel originalCategory = _dataAccess.CategoryDataAccess.GetCategory(category.Id);

            _dataAccess.CategoryDataAccess.UpdateCategory(category);

            CategoryChanged?.Invoke(this, new CategoryChangedEventArgs(originalCategory, category));
        }

        public NoteViewModel CreateNote(string noteTitle)
        {
            NoteViewModel createdNote = _dataAccess.NoteDataAccess.CreateNote(noteTitle);
            return createdNote;
        }
        public List<NoteViewModel> GetValidNotes() => _dataAccess.NoteDataAccess.GetActiveNotes();

        public NoteViewModel GetNote(int noteId) => _dataAccess.NoteDataAccess.GetNote(noteId);

        public void UpdateNote(NoteViewModel note)
        {
            if (note != null)
            {
                _dataAccess.NoteDataAccess.UpdateNote(note);
            }
        }

        /// <summary>
        /// Modifies the note order in the list to the provided <see cref="newPosition"/>.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="newPosition"></param>
        public void ReorderNote(NoteViewModel note, int newPosition)
        {
            List<NoteViewModel> activeNotes = _dataAccess.NoteDataAccess.GetActiveNotes();

            // Get all categories except the reordered one that are not trashed
            List<IReorderable> orderedNotes =
                activeNotes.Where(c => c.Id != note.Id).Cast<IReorderable>().ToList();

            IReorderable itemToReorder = note as IReorderable;

            _reorderer.ReorderItem(
                orderedNotes,
                itemToReorder,
                newPosition,
                UpdateCategoryListOrder);

            _dataAccess.NoteDataAccess.UpdateNote(note);
        }

        public void Dispose()
        {
            _dataAccess?.Dispose();
        }
    }
}