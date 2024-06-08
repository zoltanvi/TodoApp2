using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace TodoApp2.Core;

public class RecycleBinPageViewModel : BaseViewModel
{
    private AppViewModel _appViewModel;
    private TaskListService _taskListService;
    private CategoryListService _categoryListService;

    public ICommand RestoreTaskItemCommand { get; }

    public RecycleBinPageViewModel(AppViewModel appViewModel, TaskListService taskListService, CategoryListService categoryListService)
    {
        _appViewModel = appViewModel;
        _taskListService = taskListService;
        _categoryListService = categoryListService;

        RestoreTaskItemCommand = new RelayParameterizedCommand<TaskViewModel>(RestoreTaskItem);
    }

    private void RestoreTaskItem(TaskViewModel task)
    {
        var category = _categoryListService.GetCategory(task.CategoryId);
        List<TaskViewModel> taskList = _taskListService.GetActiveTaskItems(category);

        int pinnedItemCount = taskList.Count(i => i.Pinned);
        int position = task.Pinned ? 0 : pinnedItemCount;

        _taskListService.RestoreTrashedTask(task, position);
        _categoryListService.RestoreCategoryIfNeeded(task.CategoryId);
    }

}
