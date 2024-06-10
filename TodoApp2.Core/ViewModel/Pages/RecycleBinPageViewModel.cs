using MediatR;
using Modules.Categories.Contracts.Cqrs.Commands;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace TodoApp2.Core;

[AddINotifyPropertyChangedInterface]
public class RecycleBinPageViewModel : BaseViewModel
{
    private IMediator _mediator;
    private AppViewModel _appViewModel;
    private TaskListService _taskListService;

    public ICommand RestoreTaskItemCommand { get; }

    public RecycleBinPageViewModel(
        AppViewModel appViewModel,
        TaskListService taskListService,
        IMediator mediator)
    {
        _appViewModel = appViewModel;
        _taskListService = taskListService;
        _mediator = mediator;

        RestoreTaskItemCommand = new RelayParameterizedCommand<TaskViewModel>(RestoreTaskItem);
    }

    private void RestoreTaskItem(TaskViewModel task)
    {
        List<TaskViewModel> taskList = _taskListService.GetActiveTaskItems(task.CategoryId);

        int pinnedItemCount = taskList.Count(i => i.Pinned);
        int position = task.Pinned ? 0 : pinnedItemCount;

        _taskListService.RestoreTrashedTask(task, position);

        _mediator.Publish(new RestoreCategoryIfDeletedCommand() { Id = task.CategoryId });
    }
}
