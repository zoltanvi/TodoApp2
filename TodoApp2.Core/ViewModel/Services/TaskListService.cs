using Modules.Common;
using Modules.Common.ViewModel;
using Modules.Settings.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using TodoApp2.Common;
using TodoApp2.Core.Extensions;
using TodoApp2.Core.Mappings;
using TodoApp2.Core.Reordering;
using TodoApp2.Persistence;

namespace TodoApp2.Core;

/// <summary>
/// Service to hold the task item list.
/// Because this is a service, it can be accessed from multiple ViewModels.
/// </summary>
public class TaskListService : BaseViewModel
{
    private readonly CategoryListService _categoryListService;
    private readonly AppViewModel _appViewModel;
    private readonly IAppContext _context;
    private Reorderer _reorderer;
    private bool _categoryChangeInProgress;
    private bool _updateInProgress;
    private int _lastRemovedId = int.MinValue;

    private CategoryViewModel ActiveCategory => _categoryListService.ActiveCategory;
    private TaskPageSettings TaskPageSettings => AppSettings.Instance.TaskPageSettings;

    /// <summary>
    /// The task list items
    /// </summary>
    public ObservableCollection<TaskViewModel> TaskPageItems { get; }

    /// <summary>
    /// The trashed task items
    /// </summary>
    public ObservableCollection<TaskViewModel> RecycleBinItems { get; }

    public int ActiveCategoryFinishedTaskCount { get; set; }
    public int ActiveCategoryItemCount { get; set; }

    public TaskListService(IAppContext context, AppViewModel appViewModel, CategoryListService categoryListService)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(appViewModel);
        ArgumentNullException.ThrowIfNull(categoryListService);

        _context = context;
        _categoryListService = categoryListService;
        _appViewModel = appViewModel;

        _reorderer = new Reorderer();

        // Fill the actual list with the queried items
        TaskPageItems = new ObservableCollection<TaskViewModel>(GetActiveTaskItems());
        RecalculateProgress();

        // Subscribe to the collection changed event for synchronizing with database
        TaskPageItems.CollectionChanged += OnTaskPageItemsChanged;

        // Subscribe to the category changed event to filter the list when it happens
        Mediator.Register(OnCategoryChanged, ViewModelMessages.CategoryChanged);
        
        // Update recycle bin if a category is deleted
        Mediator.Register(OnCategoryDeleted, ViewModelMessages.CategoryDeleted);

        TaskPageSettings.PropertyChanged += TaskPageSettings_PropertyChanged;

        RecycleBinItems = new ObservableCollection<TaskViewModel>(GetRecycleBinItems());
    }

    public TaskViewModel AddNewTask(string taskContent)
    {
        int insertionIndex = TaskPageSettings.InsertOrderReversed
           ? TaskPageItems.Count
           : TaskPageItems.Count(i => i.Pinned);

        int activeCategoryId = _categoryListService.ActiveCategory.Id;

        var task = CreateTask(taskContent, activeCategoryId);
        var correctedInsertionIndex = GetReorderIndex(insertionIndex, task, TaskPageItems, true);

        // Sets the correct ListOrder on the task
        _reorderer.ReorderItem(
            TaskPageItems.Cast<IReorderable>().ToList(),
            task,
            correctedInsertionIndex,
            UpdateListOrder);

        var addedItem = _context.Tasks.Add(task.Map());
        var createdTask = addedItem.Map();

        TaskPageItems.Insert(correctedInsertionIndex, createdTask);
        RecalculateProgress();

        return createdTask;
    }

    private TaskViewModel CreateTask(string taskContent, int categoryId)
    {
        TaskViewModel task = new TaskViewModel
        {
            CategoryId = categoryId,
            Content = taskContent,
            CreationDate = DateTime.Now.Ticks,
            ModificationDate = DateTime.Now.Ticks,
            Color = Constants.ColorName.Transparent,
            BorderColor = Constants.ColorName.Transparent,
            BackgroundColor = Constants.ColorName.Transparent,
        };

        return task;
    }

    public void RestoreTrashedTask(TaskViewModel task, int oldPosition)
    {
        task.Trashed = false;

        ReorderTask(task, oldPosition, false);

        RecycleBinItems.Remove(task);
    }

    public TaskViewModel UntrashExistingTask(TaskViewModel task, int oldPosition)
    {
        task.Trashed = false;

        // The task exist in the database but not in the list.
        TaskPageItems.Insert(0, task);

        ReorderTask(task, oldPosition, true);
        RecalculateProgress();

        return task;
    }

    public void UpdateTask(TaskViewModel task)
    {
        _updateInProgress = true;

        if (task.Trashed || (ActiveCategory != null && task.CategoryId != ActiveCategory.Id))
        {
            TaskPageItems.Remove(task);
        }

        _context.Tasks.UpdateFirst(task.Map());

        // Update task in collection
        var pageTask = TaskPageItems.FirstOrDefault(x => x.Id == task.Id);
        if (pageTask != null && !ReferenceEquals(task, pageTask))
        {
            var index = TaskPageItems.IndexOf(pageTask);
            var updatedTask = _context.Tasks.First(x => x.Id == task.Id);

            if (updatedTask == null) throw new ApplicationException($"Couldn't update task with ID [{task.Id}].");

            TaskPageItems.RemoveAt(index);
            TaskPageItems.Insert(index, updatedTask.Map());
        }

        RecalculateProgress();

        _updateInProgress = false;
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

        if (task != null && ActiveCategory != null)
        {
            if (changeInCollection)
            {
                var oldIndex = TaskPageItems.IndexOf(task);
                TaskPageItems.Move(oldIndex, newPosition);
            }

            if (task.CategoryId != ActiveCategory.Id) TaskPageItems.Remove(task);

            ReorderingHelperTemp.ReorderTask(_context, task, newPosition);
        }
    }

    public void PersistTaskList(IEnumerable<TaskViewModel> taskList)
    {
        _context.Tasks.UpdateRange(taskList.MapList(), x => x.Id);
    }

    public void PersistTaskList()
    {
        _context.Tasks.UpdateRange(TaskPageItems.MapList(), x => x.Id);

        foreach (var item in TaskPageItems.Where(x => x.Trashed).ToList())
        {
            TaskPageItems.Remove(item);
        }

        RecalculateProgress();
    }

    public List<TaskViewModel> GetActiveTaskItems(CategoryViewModel category)
    {
        return _context.Tasks
        .Where(x => x.CategoryId == category.Id && !x.Trashed)
        .OrderByListOrder()
        .MapList();
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
            var categoryTasks = _context.Tasks
                .Where(x => x.CategoryId == category.Id && !x.Trashed)
                .OrderByListOrder()
                .MapList();

            newIndex = GetReorderIndex(newIndex, task, categoryTasks);
        }

        return newIndex;
    }

    public void SortByState() => SortItemsByState(TaskPageItems);

    public void SortByCreationDate() => RebuildListOrders(TaskPageItems.OrderBy(x => x.CreationDate));
    public void SortByCreationDateDesc() => RebuildListOrders(TaskPageItems.OrderByDescending(x => x.CreationDate));
    public void SortByModificationDate() => RebuildListOrders(TaskPageItems.OrderBy(x => x.ModificationDate));
    public void SortByModificationDateDesc() => RebuildListOrders(TaskPageItems.OrderByDescending(x => x.ModificationDate));

    private void TaskPageSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TaskPageSettings.ForceTaskOrderByState) && TaskPageSettings.ForceTaskOrderByState)
        {
            SortItemsByState(TaskPageItems);
        }
    }

    private void SortItemsByState(IEnumerable<TaskViewModel> originalTaskList)
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

    private void RebuildListOrders(IEnumerable<TaskViewModel> taskList)
    {
        var tempList = taskList.ToList();
        TaskPageItems.Clear();
        TaskPageItems.AddRange(tempList);

        _reorderer.ResetListOrders(TaskPageItems);
        PersistTaskList();
    }

    private void OnCategoryChanged(object obj)
    {
        _categoryChangeInProgress = true;

        // Clear the list first to prevent inconsistent data on UI while the items are loading
        TaskPageItems.Clear();
        RecycleBinItems.Clear();

        // Query the items with the current category
        List<TaskViewModel> filteredItems = GetActiveTaskItems();
        List<TaskViewModel> trashedItems = GetRecycleBinItems();

        // Clear the list to prevent showing items from multiple categories.
        // This can happen if the user changes category again while the query runs
        TaskPageItems.Clear();
        RecycleBinItems.Clear();

        // Abort the previous task list loading
        IoC.AsyncActionService.AbortRunningActions();

        RecycleBinItems.AddRange(trashedItems);

        // Fill the actual list with the queried items
        if (TaskPageSettings.ForceTaskOrderByState)
        {
            SortItemsByState(filteredItems);
        }
        else
        {
            TaskPageItems.AddRange(filteredItems);

            //Action AddItem = new Action(() => TaskPageItems.Add(item));
            //IoC.AsyncActionService.InvokeAsync(AddItem);
        }

        RecalculateProgress();
        _categoryChangeInProgress = false;
    }

    private void OnCategoryDeleted(object obj)
    {
        // Clear the list first to prevent inconsistent data on UI while the items are loading
        RecycleBinItems.Clear();

        // Query the items with the current category
        List<TaskViewModel> trashedItems = GetRecycleBinItems();

        // Clear the list to prevent showing items from multiple categories.
        // This can happen if the user changes category again while the query runs
        RecycleBinItems.Clear();

        RecycleBinItems.AddRange(trashedItems);
    }

    private int GetReorderIndex(
        int newIndex,
        TaskViewModel task,
        IEnumerable<TaskViewModel> taskList,
        bool insertion = false)
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
                    newIndex = listCount - doneItemsCount;

                    // Subtract self if it is a reorder and not an insertion
                    if (!insertion) newIndex--;
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
        if (_updateInProgress || (_categoryChangeInProgress && !TaskPageSettings.ForceTaskOrderByState))
        {
            return;
        }

        if (e.Action == NotifyCollectionChangedAction.Add && HasNewItems())
        {
            var newItem = GetNewTask();

            // If the newly added item is the same as the last deleted one,
            // then this was a drag and drop reorder
            if (newItem.Id == _lastRemovedId)
            {
                ReorderTask(newItem, e.NewStartingIndex);
            }

            _lastRemovedId = int.MinValue;
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && HasOldItems())
        {
            _lastRemovedId = GetOldTask().Id;
        }
        else if (e.Action == NotifyCollectionChangedAction.Move && HasNewItems())
        {
            ReorderTask(GetNewTask(), e.NewStartingIndex);
        }

        TaskViewModel GetNewTask() => Get(e.NewItems);
        TaskViewModel GetOldTask() => Get(e.OldItems);
        TaskViewModel Get(IList list) => (list.Count > 0) ? (TaskViewModel)list[0] : null;
        bool HasNewItems() => HasItems(e.NewItems);
        bool HasOldItems() => HasItems(e.OldItems);
        bool HasItems(IList list) => list.Count > 0;
    }

    private void UpdateListOrder(IEnumerable<IReorderable> taskList) =>
        _context.Tasks.UpdateRange(taskList.Cast<TaskViewModel>().MapList(), x => x.Id);

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

    private void RecalculateProgress()
    {
        ActiveCategoryItemCount = TaskPageItems.Count(i => !i.Trashed);
        ActiveCategoryFinishedTaskCount = TaskPageItems.Count(x => !x.Trashed && x.IsDone);
    }

    private List<TaskViewModel> GetActiveTaskItems()
    {
        if (ActiveCategory?.Id != CommonConstants.RecycleBinCategoryId)
        {
            return _context.Tasks
            .Where(x => x.CategoryId == ActiveCategory.Id && !x.Trashed)
            .OrderByListOrder()
            .MapList();
        }

        return new List<TaskViewModel>();
    }

    private List<TaskViewModel> GetRecycleBinItems()
    {
        if (ActiveCategory?.Id == CommonConstants.RecycleBinCategoryId)
        {
            return _context.Tasks
                .Where(x => x.Trashed)
                .OrderByDescending(x => x.TrashedDate)
                .MapList();
        }

        return new List<TaskViewModel>();
    }
}
