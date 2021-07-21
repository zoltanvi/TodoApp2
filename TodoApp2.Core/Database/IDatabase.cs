﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    public interface IDatabase
    {
        event EventHandler<TaskChangedEventArgs> TaskChanged;

        Task Reinitialize(bool online = false);
        bool AddCategoryIfNotExists(CategoryListItemViewModel categoryToAdd);
        void AddSetting(SettingsModel setting);
        void AddTask(TaskListItemViewModel task);
        void Dispose();
        List<CategoryListItemViewModel> GetActiveCategories();
        List<TaskListItemViewModel> GetActiveTaskItems();
        List<TaskListItemViewModel> GetActiveTaskItems(string categoryName);
        Task<List<TaskListItemViewModel>> GetActiveTaskItemsAsync(string categoryName);
        CategoryListItemViewModel GetCategory(int categoryId);
        CategoryListItemViewModel GetCategory(string categoryName);
        List<SettingsModel> GetSettings();
        TaskListItemViewModel GetTask(int id);
        void ReorderCategory(CategoryListItemViewModel category, int newPosition);
        void ReorderTask(TaskListItemViewModel task, int newPosition);
        void TrashCategory(CategoryListItemViewModel category);
        void UntrashCategory(CategoryListItemViewModel category);
        void UpdateSettings(List<SettingsModel> settings);
        void UpdateCategoryListOrder(IEnumerable<IReorderable> categoryList);
        void UpdateTask(TaskListItemViewModel task);
        void UpdateTaskList(IEnumerable<IReorderable> taskList);
        void UpdateTaskListOrder(IEnumerable<IReorderable> taskList);
    }
}