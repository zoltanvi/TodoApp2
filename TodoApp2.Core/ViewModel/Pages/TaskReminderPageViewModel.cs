using Modules.Common.DataBinding;
using Modules.Common.DataModels;
using Modules.Common.Navigation;
using Modules.Common.ViewModel;
using System;
using System.Windows.Input;
using TodoApp2.Persistence;

namespace TodoApp2.Core;

public class TaskReminderPageViewModel : BaseViewModel
{
    private readonly IAppContext _context;

    /// <summary>
    /// The task to show the notification for.
    /// </summary>
    public TaskViewModel ReminderTask => IoC.OverlayPageService.Task;

    public long ReminderDateTime => ReminderTask?.ReminderDate ?? 0;

    public bool IsReminderOnToggle => ReminderDateTime > 0;

    public bool HasValidReminderTime => ReminderDateTime != 0; 

    public bool IsReminderOn
    {
        get => ReminderTask.IsReminderOn;
        set => ReminderTask.IsReminderOn = value;
    }

    public ICommand ClosePageCommand { get; }
    public ICommand EditReminderCommand { get; }
    public ICommand ChangeIsReminderOn { get; }


    public TaskReminderPageViewModel(IAppContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        _context = context;

        EditReminderCommand = new RelayCommand(EditReminder);
        ClosePageCommand = new RelayCommand(ClosePage);
        ChangeIsReminderOn = new RelayCommand(ChangeIsOn);
        IoC.OverlayPageService.SetBackgroundClickedAction(ClosePage);
    }

    private void ChangeIsOn()
    {
        // ReminderTask.IsReminderOn is modified with the toggle button,
        // so we persist the modification
        IoC.TaskListService.UpdateTask(ReminderTask);
    }

    private void EditReminder()
    {
        IoC.AppViewModel.OpenOverlayPage<ITaskReminderEditorPage>(ReminderTask);
    }

    private void ClosePage()
    {
        IoC.OverlayPageService.ClosePage();
    }
}