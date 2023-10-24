using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    public interface IDatabase
    {
        event EventHandler<TaskChangedEventArgs> TaskChanged;
        event EventHandler<CategoryChangedEventArgs> CategoryChanged;

        void AddDefaultRecords();
        bool AddCategoryIfNotExists(CategoryViewModel categoryToAdd);
        void Dispose();
        List<CategoryViewModel> GetValidCategories();
        List<TaskViewModel> GetActiveTasksWithReminder();
        List<TaskViewModel> GetActiveTasks(CategoryViewModel category);
        Task<List<TaskViewModel>> GetActiveTasksAsync(CategoryViewModel category);
        CategoryViewModel GetCategory(int categoryId);
        CategoryViewModel GetCategory(string categoryName);
        List<Setting> GetSettings();
        TaskViewModel GetTask(int id);
        void ReorderCategory(CategoryViewModel category, int newPosition);
        void ReorderTask(TaskViewModel task, int newPosition);
        void TrashCategory(CategoryViewModel category);
        void UntrashCategory(CategoryViewModel category);
        void UpdateCategory(CategoryViewModel category);
        void UpdateSettings(List<Setting> settings);
        void UpdateCategoryListOrder(IEnumerable<IReorderable> categoryList);
        void UpdateTask(TaskViewModel task);
        void UpdateTasks(IEnumerable<IReorderable> taskList);
        void UpdateTaskListOrder(IEnumerable<IReorderable> taskList);
        TaskViewModel CreateTask(string taskContent, int categoryId, int position);
        NoteViewModel CreateNote(string noteTitle);
        void UpdateNote(NoteViewModel note);
        List<NoteViewModel> GetValidNotes();
        void ReorderNote(NoteViewModel newItem, int newStartingIndex);
        NoteViewModel GetNote(int noteId);
    }
}