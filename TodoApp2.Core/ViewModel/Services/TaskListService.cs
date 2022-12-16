using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Threading;

namespace TodoApp2.Core
{
    /// <summary>
    /// Service to hold the task item list.
    /// Because this is a service, it can be accessed from multiple ViewModels.
    /// </summary>
    public class TaskListService : BaseViewModel
    {
        private int m_LastRemovedId = int.MinValue;
        private readonly CategoryListService m_CategoryListService;
        private readonly ApplicationViewModel m_ApplicationViewModel;
        private readonly IDatabase m_Database;

        private CategoryListItemViewModel ActiveCategory => m_CategoryListService.ActiveCategory;

        /// <summary>
        /// The task list items
        /// </summary>
        public ObservableCollection<TaskListItemViewModel> TaskPageItems { get; }

        public TaskListService(IDatabase database, CategoryListService categoryListService, ApplicationViewModel applicationViewModel)
        {
            m_Database = database;
            m_CategoryListService = categoryListService;
            m_ApplicationViewModel = applicationViewModel;

            // Query the items with the current category
            List<TaskListItemViewModel> items = m_Database.GetActiveTaskItems(ActiveCategory);

            // Fill the actual list with the queried items
            TaskPageItems = new ObservableCollection<TaskListItemViewModel>(items);

            // Subscribe to the collection changed event for synchronizing with database
            TaskPageItems.CollectionChanged += OnTaskPageItemsChanged;

            m_Database.TaskChanged += OnClientDatabaseTaskChanged;

            // Subscribe to the category changed event to filter the list when it happens
            Mediator.Register(OnCategoryChanged, ViewModelMessages.CategoryChanged);
            Mediator.Register(OnOnlineModeChangeRequested, ViewModelMessages.OnlineModeChangeRequested);
            Mediator.Register(OnOnlineModeChanged, ViewModelMessages.OnlineModeChanged);
        }

        private void OnOnlineModeChangeRequested(object obj)
        {
            PersistTaskList();
        }

        private void OnOnlineModeChanged(object obj)
        {
            TaskPageItems.Clear();
            List<TaskListItemViewModel> items = m_Database.GetActiveTaskItems(ActiveCategory);
            TaskPageItems.AddRange(items);
        }

        public TaskListItemViewModel AddNewTask(string taskContent)
        {
            TaskListItemViewModel task;
            int pinnedItemsCount = TaskPageItems.Count(i => i.Pinned);
            int activeCategoryId = m_CategoryListService.ActiveCategory.Id;

            if (m_ApplicationViewModel.ApplicationSettings.InsertOrderReversed)
            {
                int unfinishedTaskCount = TaskPageItems.Count(i => !i.IsDone);

                // Insert the task at the end of the list
                task = m_Database.CreateTask(taskContent, activeCategoryId, unfinishedTaskCount);
                TaskPageItems.Insert(unfinishedTaskCount, task);
            }
            else
            {
                // Insert the task at the beginning of the list
                task = m_Database.CreateTask(taskContent, activeCategoryId, pinnedItemsCount);
                TaskPageItems.Insert(pinnedItemsCount, task);
            }

            return task;
        }

        public TaskListItemViewModel UntrashExistingTask(TaskListItemViewModel task, int oldPosition)
        {
            task.Trashed = false;

            // The task exist in the database but not in the list.
            TaskPageItems.Insert(0, task);

            ReorderTask(task, oldPosition, true);

            return task;
        }

        public void UpdateTask(TaskListItemViewModel task)
        {
            TaskListItemViewModel taskToUpdate = TaskPageItems.FirstOrDefault(item => item.Id == task.Id);
            taskToUpdate?.CopyProperties(task);
            m_Database.UpdateTask(task);
        }

        /// <inheritdoc cref="Core.Database.ReorderTask"/>
        /// <param name="changeInCollection">
        /// If true, the item is moved in the collection also.
        /// If false, the new position is only saved in the database.
        /// Do not call it with true during handling the collection changed event.
        /// </param>
        public void ReorderTask(TaskListItemViewModel task, int newPosition, bool changeInCollection = false)
        {
            newPosition = newPosition == -1 ? 0 : newPosition;

            TaskListItemViewModel taskToUpdate = TaskPageItems.FirstOrDefault(item => item.Id == task.Id);

            if (taskToUpdate != null)
            {
                taskToUpdate.CopyProperties(task);

                if (changeInCollection)
                {
                    var oldIndex = TaskPageItems.IndexOf(task);
                    TaskPageItems.Move(oldIndex, newPosition);
                }

                m_Database.ReorderTask(task, newPosition);
            }
        }

        public void RemoveTaskFromMemory(TaskListItemViewModel task)
        {
            TaskPageItems.Remove(task);
        }

        public void PersistTaskList()
        {
            m_Database.UpdateTaskList(TaskPageItems);
        }

        /// <inheritdoc cref="Core.Database.GetActiveTaskItemsAsync"/>
        public async Task<List<TaskListItemViewModel>> GetActiveTaskItemsAsync(CategoryListItemViewModel category)
        {
            return await m_Database.GetActiveTaskItemsAsync(category);
        }

        private void OnClientDatabaseTaskChanged(object sender, TaskChangedEventArgs e)
        {
            TaskListItemViewModel modifiedItem = TaskPageItems.FirstOrDefault(item => item.Id == e.Task.Id);

            modifiedItem?.CopyProperties(e.Task);
        }

        private async Task OnCategoryChanged()
        {
            // Clear the list first to prevent inconsistent data on UI while the items are loading
            TaskPageItems.Clear();

            // Query the items with the current category
            List<TaskListItemViewModel> filteredItems = await GetActiveTaskItemsAsync(ActiveCategory);

            // Clear the list to prevent showing items from multiple categories.
            // This can happen if the user changes category again while the query runs
            TaskPageItems.Clear();

            // Fill the actual list with the queried items
            foreach (TaskListItemViewModel item in filteredItems)
            {
                // TODO: Stop the process on category change
                //Action AddItem = new Action(() => TaskPageItems.Add(item));
                //IoC.AsyncActionService.InvokeAsync(AddItem);

                TaskPageItems.Add(item);
            }
        }

        /// <summary>
        /// Returns the correct reorder index of the <paramref name="task"/>
        /// in the currently active category task list.
        /// </summary>
        /// <param name="newIndex">The index where the task should be moved.</param>
        /// <param name="task">The task to reorder.</param>
        /// <returns> the correct reorder index where the task can be moved.</returns>
        public int GetCorrectReorderIndex(int newIndex, TaskListItemViewModel task)
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
        public int GetCorrectReorderIndex(int newIndex, TaskListItemViewModel task, CategoryListItemViewModel category)
        {
            if (task != null)
            {
                List<TaskListItemViewModel> categoryTasks = m_Database.GetActiveTaskItems(category);
                newIndex = GetReorderIndex(newIndex, task, categoryTasks);
            }

            return newIndex;
        }

        private int GetReorderIndex(int newIndex, TaskListItemViewModel task,
            IEnumerable<TaskListItemViewModel> taskList)
        {
            if (task != null)
            {
                var pinnedItemsCount = taskList.Count(i => i.Pinned);

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
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    if (e.NewItems.Count > 0)
                    {
                        TaskListItemViewModel newItem = (TaskListItemViewModel)e.NewItems[0];

                        // If the newly added item is the same as the last deleted one,
                        // then this was a drag and drop reorder
                        if (newItem.Id == m_LastRemovedId)
                        {
                            ReorderTask(newItem, e.NewStartingIndex);
                        }

                        m_LastRemovedId = int.MinValue;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems.Count > 0)
                    {
                        TaskListItemViewModel last = (TaskListItemViewModel)e.OldItems[0];

                        m_LastRemovedId = last.Id;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Move:
                {
                    if (e.NewItems.Count > 0)
                    {
                        TaskListItemViewModel newItem = (TaskListItemViewModel)e.NewItems[0];

                        ReorderTask(newItem, e.NewStartingIndex);
                    }
                    break;
                }
            }
        }
    }
}
