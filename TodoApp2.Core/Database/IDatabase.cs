using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    public interface IDatabase
    {
        event EventHandler<TaskChangedEventArgs> TaskChanged;
        event EventHandler<CategoryChangedEventArgs> CategoryChanged;

        Task Initialize();
        bool AddCategoryIfNotExists(CategoryListItemViewModel categoryToAdd);
        void AddSetting(SettingsModel setting);
        void Dispose();
        List<CategoryListItemViewModel> GetValidCategories();
        List<TaskListItemViewModel> GetActiveTaskItems();
        List<TaskListItemViewModel> GetActiveTaskItemsWithReminder();
        List<TaskListItemViewModel> GetActiveTaskItems(CategoryListItemViewModel category);
        Task<List<TaskListItemViewModel>> GetActiveTaskItemsAsync(CategoryListItemViewModel category);
        CategoryListItemViewModel GetCategory(int categoryId);
        CategoryListItemViewModel GetCategory(string categoryName);
        List<SettingsModel> GetSettings();
        TaskListItemViewModel GetTask(int id);
        void ReorderCategory(CategoryListItemViewModel category, int newPosition);
        void ReorderTask(TaskListItemViewModel task, int newPosition);
        void TrashCategory(CategoryListItemViewModel category);
        void UntrashCategory(CategoryListItemViewModel category);
        void UpdateCategory(CategoryListItemViewModel category);
        void UpdateSettings(List<SettingsModel> settings);
        void UpdateCategoryListOrder(IEnumerable<IReorderable> categoryList);
        void UpdateTask(TaskListItemViewModel task);
        void UpdateTaskList(IEnumerable<IReorderable> taskList);
        void UpdateTaskListOrder(IEnumerable<IReorderable> taskList);
        TaskListItemViewModel CreateTask(string taskContent, int categoryId, int position);
    }
}