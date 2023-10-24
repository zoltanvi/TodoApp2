﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    /// <summary>
    /// Service to hold the task item list.
    /// Because this is a service, it can be accessed from multiple ViewModels.
    /// </summary>
    public class TaskListService : BaseViewModel
    {
        private readonly CategoryListService _categoryListService;
        private readonly AppViewModel _appViewModel;
        private readonly IDatabase _database;
        private Reorderer _reorderer;
        private bool _categoryChangeInProgress;
        private int _lastRemovedId = int.MinValue;

        private CategoryViewModel ActiveCategory => _categoryListService.ActiveCategory;
        private TaskPageSettings TaskPageSettings => IoC.AppSettings.TaskPageSettings;

        /// <summary>
        /// The task list items
        /// </summary>
        public ObservableCollection<TaskViewModel> TaskPageItems { get; }

        public TaskListService(IDatabase database, CategoryListService categoryListService, AppViewModel applicationViewModel)
        {
            _database = database;
            _categoryListService = categoryListService;
            _appViewModel = applicationViewModel;
            _reorderer = new Reorderer();
            // Query the items with the current category
            List<TaskViewModel> items = _database.GetActiveTasks(ActiveCategory);

            // Fill the actual list with the queried items
            TaskPageItems = new ObservableCollection<TaskViewModel>(items);

            // Subscribe to the collection changed event for synchronizing with database
            TaskPageItems.CollectionChanged += OnTaskPageItemsChanged;

            _database.TaskChanged += OnClientDatabaseTaskChanged;

            // Subscribe to the category changed event to filter the list when it happens
            Mediator.Register(OnCategoryChanged, ViewModelMessages.CategoryChanged);

            TaskPageSettings.PropertyChanged += TaskPageSettings_PropertyChanged;
        }

        public TaskViewModel AddNewTask(string taskContent)
        {
            TaskViewModel task;
            int pinnedItemsCount = TaskPageItems.Count(i => i.Pinned);
            int activeCategoryId = _categoryListService.ActiveCategory.Id;

            if (TaskPageSettings.InsertOrderReversed)
            {
                int endInsertIndex = TaskPageItems.Count - CountDoneItemsFromBackwards();

                // Insert the task at the end of the list
                task = _database.CreateTask(taskContent, activeCategoryId, endInsertIndex);
                TaskPageItems.Insert(endInsertIndex, task);
            }
            else
            {
                // Insert the task at the beginning of the list
                task = _database.CreateTask(taskContent, activeCategoryId, pinnedItemsCount);
                TaskPageItems.Insert(pinnedItemsCount, task);
            }

            return task;
        }

        public TaskViewModel UntrashExistingTask(TaskViewModel task, int oldPosition)
        {
            task.Trashed = false;

            // The task exist in the database but not in the list.
            TaskPageItems.Insert(0, task);

            ReorderTask(task, oldPosition, true);

            return task;
        }

        public void UpdateTask(TaskViewModel task)
        {
            TaskViewModel taskToUpdate = TaskPageItems.FirstOrDefault(item => item.Id == task.Id);
            taskToUpdate?.CopyProperties(task);
            _database.UpdateTask(task);
        }

        /// <inheritdoc cref="Core.Database.ReorderTask"/>
        /// <param name="changeInCollection">
        /// If true, the item is moved in the collection also.
        /// If false, the new position is only saved in the database.
        /// Do not call it with true during handling the collection changed event.
        /// </param>
        public void ReorderTask(TaskViewModel task, int newPosition, bool changeInCollection = false)
        {
            newPosition = newPosition == -1 ? 0 : newPosition;

            TaskViewModel taskToUpdate = TaskPageItems.FirstOrDefault(item => item.Id == task.Id);

            if (taskToUpdate != null)
            {
                taskToUpdate.CopyProperties(task);

                if (changeInCollection)
                {
                    var oldIndex = TaskPageItems.IndexOf(task);
                    TaskPageItems.Move(oldIndex, newPosition);
                }

                _database.ReorderTask(task, newPosition);
            }
        }

        public void RemoveTaskFromMemory(TaskViewModel task)
        {
            TaskPageItems.Remove(task);
        }

        public void PersistTaskList()
        {
            _database.UpdateTasks(TaskPageItems);
        }

        /// <inheritdoc cref="Database.GetActiveTasksAsync"/>
        public async Task<List<TaskViewModel>> GetActiveTaskItemsAsync(CategoryViewModel category)
        {
            return await _database.GetActiveTasksAsync(category);
        }

        /// <summary>
        /// Returns the correct reorder index of the <paramref name="task"/>
        /// in the currently active category task list.
        /// </summary>
        /// <param name="newIndex">The index where the task should be moved.</param>
        /// <param name="task">The task to reorder.</param>
        /// <returns> the correct reorder index where the task can be moved.</returns>
        public int GetCorrectReorderIndex(int newIndex, TaskViewModel task)
        {
            return GetReorderIndex(newIndex, task, TaskPageItems);
        }

        /// <summary>
        /// Returns the correct reorder index of the <paramref name="task"/>
        /// in the <paramref name="categoryName"/> category task list.
        /// </summary>
        /// <param name="newIndex">The index where the task should be moved.</param>
        /// <param name="task">The task to reorder.</param>
        /// <param name="categoryName">The category where the task should be moved.</param>
        /// <returns> the correct reorder index where the task can be moved.</returns>
        public int GetCorrectReorderIndex(int newIndex, TaskViewModel task, CategoryViewModel category)
        {
            if (task != null)
            {
                List<TaskViewModel> categoryTasks = _database.GetActiveTasks(category);
                newIndex = GetReorderIndex(newIndex, task, categoryTasks);
            }

            return newIndex;
        }

        public void SortTaskPageItems() => FixTaskPageItemsOrder(TaskPageItems);

        private void TaskPageSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TaskPageSettings.ForceTaskOrderByState) && TaskPageSettings.ForceTaskOrderByState)
            {
                FixTaskPageItemsOrder(TaskPageItems);
            }
        }

        private void FixTaskPageItemsOrder(IEnumerable<TaskViewModel> originalTaskList)
        {
            // Sort and reorder tasks
            List<TaskViewModel> originalList = originalTaskList.ToList();
            TaskPageItems.Clear();

            IEnumerable<TaskViewModel> pinnedItems = originalList.Where(t => t.Pinned && !t.IsDone);
            IEnumerable<TaskViewModel> unfinishedItems = originalList.Where(t => !t.Pinned && !t.IsDone);
            IEnumerable<TaskViewModel> finishedItems = originalList.Where(t => !t.Pinned && t.IsDone);

            TaskPageItems.AddRange(pinnedItems);
            TaskPageItems.AddRange(unfinishedItems);
            TaskPageItems.AddRange(finishedItems);

            _reorderer.ResetListOrders(TaskPageItems);
            PersistTaskList();
        }

        private void OnClientDatabaseTaskChanged(object sender, TaskChangedEventArgs e)
        {
            TaskViewModel modifiedItem = TaskPageItems.FirstOrDefault(item => item.Id == e.Task.Id);

            modifiedItem?.CopyProperties(e.Task);
        }

        private async Task OnCategoryChanged()
        {
            _categoryChangeInProgress = true;

            // Clear the list first to prevent inconsistent data on UI while the items are loading
            TaskPageItems.Clear();

            // Query the items with the current category
            List<TaskViewModel> filteredItems = await GetActiveTaskItemsAsync(ActiveCategory);

            // Clear the list to prevent showing items from multiple categories.
            // This can happen if the user changes category again while the query runs
            TaskPageItems.Clear();

            // Abort the previous task list loading
            IoC.AsyncActionService.AbortRunningActions();

            // Fill the actual list with the queried items
            if (TaskPageSettings.ForceTaskOrderByState)
            {
                FixTaskPageItemsOrder(filteredItems);
            }
            else
            {
                TaskPageItems.AddRange(filteredItems);

                //Action AddItem = new Action(() => TaskPageItems.Add(item));
                //IoC.AsyncActionService.InvokeAsync(AddItem);
            }

            _categoryChangeInProgress = false;
        }

        private int GetReorderIndex(int newIndex, TaskViewModel task,
            IEnumerable<TaskViewModel> taskList)
        {
            if (task != null)
            {
                CountTasks(taskList, out int listCount, out int pinnedItemsCount, out int doneItemsCount);

                // If the task is pinned,
                // it must be on top of the list or directly before or after another pinned item
                if (task.Pinned && newIndex > pinnedItemsCount - 1)
                {
                    newIndex = pinnedItemsCount - 1;
                }
                // If the task is not pinned, it must be after the pinned tasks.
                else if (!task.Pinned && newIndex < pinnedItemsCount)
                {
                    newIndex = pinnedItemsCount;
                }

                if (TaskPageSettings.ForceTaskOrderByState)
                {
                    // If the task is done,
                    // it must be on the bottom of the list or directly before or after another done item
                    if (task.IsDone && newIndex < listCount - doneItemsCount)
                    {
                        newIndex = listCount - doneItemsCount;
                    }
                    // If the task is not done, it must be before the finished tasks.
                    else if (!task.IsDone && newIndex > listCount - doneItemsCount - 1)
                    {
                        newIndex = listCount - doneItemsCount - 1;
                    }
                }
            }

            return newIndex;
        }

        /// <summary>
        /// Called when the Items collection changes.
        /// We want to synchronize the list order when it happens.
        /// Note: The Drag & Drop causes a Remove and Add action sequence.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTaskPageItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_categoryChangeInProgress && !TaskPageSettings.ForceTaskOrderByState)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    if (e.NewItems.Count > 0)
                    {
                        TaskViewModel newItem = (TaskViewModel)e.NewItems[0];

                        // If the newly added item is the same as the last deleted one,
                        // then this was a drag and drop reorder
                        if (newItem.Id == _lastRemovedId)
                        {
                            ReorderTask(newItem, e.NewStartingIndex);
                        }

                        _lastRemovedId = int.MinValue;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems.Count > 0)
                    {
                        TaskViewModel last = (TaskViewModel)e.OldItems[0];

                        _lastRemovedId = last.Id;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Move:
                {
                    if (e.NewItems.Count > 0)
                    {
                        TaskViewModel newItem = (TaskViewModel)e.NewItems[0];

                        ReorderTask(newItem, e.NewStartingIndex);
                    }
                    break;
                }
            }
        }

        private void CountTasks(
            IEnumerable<TaskViewModel> taskList,
            out int listCount,
            out int pinnedItemsCount,
            out int doneItemsCount)
        {
            listCount = 0;
            pinnedItemsCount = 0;
            doneItemsCount = 0;

            foreach (TaskViewModel task in taskList)
            {
                listCount++;

                if (task.IsDone)
                {
                    doneItemsCount++;
                }

                if (task.Pinned)
                {
                    pinnedItemsCount++;
                }
            }
        }

        /// <summary>
        /// Counts the done tasks in the list which are at the end of the list.
        /// </summary>
        private int CountDoneItemsFromBackwards()
        {
            int count = 0;
            for (int i = TaskPageItems.Count - 1; i >= 0; i--)
            {
                if (!TaskPageItems[i].IsDone)
                {
                    break;
                }

                count++;
            }

            return count;
        }
    }
}
