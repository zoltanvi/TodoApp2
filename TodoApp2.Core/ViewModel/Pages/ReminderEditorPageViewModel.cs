using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TodoApp2.Core.Extensions;
using TodoApp2.Persistence;

namespace TodoApp2.Core;

public class ReminderEditorPageViewModel : BaseViewModel
{
    private bool _closed;

    private readonly IAppContext _context;

    public TimePickerViewModel TimePickerViewModel { get; set; }

    /// <summary>
    /// The task to show the notification for.
    /// </summary>
    public TaskViewModel ReminderTask => IoC.OverlayPageService.Task;
    public DateTime SelectedDate { get; set; }
    public string SelectedDateString { get; set; }
    public bool IsSelectedDateStringValid { get; set; }
    public bool IsReminderOn
    {
        get => ReminderTask.IsReminderOn;
        set => ReminderTask.IsReminderOn = value;
    }

    #region Workaround
    // WORKAROUND properties for MultiBinding bug
    // See: https://stackoverflow.com/questions/22536645/what-hardware-platform-difference-could-cause-an-xaml-wpf-multibinding-to-checkb
    public double MyWidth { get; set; }
    public double MyHeight { get; set; }
    public Rect ClipRect => new Rect(0, 0, MyWidth, MyHeight);
    #endregion Workaround

    public ICommand CloseReminderCommand { get; }
    public ICommand SetReminderCommand { get; }
    public ICommand ResetReminderCommand { get; }
    public ICommand ChangeIsReminderOn { get; }

    public ReminderEditorPageViewModel(IAppContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;

        SetReminderCommand = new RelayCommand(SetReminder);
        ResetReminderCommand = new RelayCommand(ResetReminder);
        CloseReminderCommand = new RelayCommand(ClosePage);
        ChangeIsReminderOn = new RelayCommand(ChangeReminder);
        IoC.OverlayPageService.SetBackgroundClickedAction(ClosePage);

        TimePickerViewModel = new TimePickerViewModel();

        ResetReminderProperties();

        IsSelectedDateStringValid = true;

        PropertyChanged += OnViewModelPropertyChanged;
    }

    private void SetReminder()
    {
        IsReminderOn = true;
        SetReminderDate();
        IoC.TaskListService.UpdateTask(ReminderTask);

        IoC.ReminderNotificationService.SetReminder(ReminderTask);

        ClosePage();
    }

    private void ResetReminder()
    {
        ReminderTask.ReminderDate = 0;
        ResetReminderProperties();
        IsReminderOn = false;

        IoC.TaskListService.UpdateTask(ReminderTask);
        IoC.ReminderNotificationService.DeleteReminder(ReminderTask);
    }

    private void ChangeReminder()
    {
        SetReminderDate();

        IoC.TaskListService.UpdateTask(ReminderTask);
    }

    private void ClosePage()
    {
        if (!_closed)
        {
            _closed = true;

            PropertyChanged -= OnViewModelPropertyChanged;

            IoC.OverlayPageService.ClosePage();
        }
    }

    private void SetReminderDate()
    {
        DateTime reminderDate = SelectedDate.Date + new TimeSpan(TimePickerViewModel.Hour, TimePickerViewModel.Minute, 0);
        ReminderTask.ReminderDate = reminderDate.Ticks;
    }

    private void ResetReminderProperties()
    {
        if (ReminderTask.ReminderDate != 0)
        {
            DateTime date = new DateTime(ReminderTask.ReminderDate);
            SelectedDate = date.Date;
            TimePickerViewModel.Hour = date.Hour;
            TimePickerViewModel.Minute = date.Minute;
            SelectedDateString = SelectedDate.ConvertToString();
        }
        else
        {
            DateTime fiveMinutesFromCurrentTime = DateTime.Now.AddMinutes(5);
            SelectedDateString = fiveMinutesFromCurrentTime.ConvertToString();
            SelectedDate = fiveMinutesFromCurrentTime;
            TimePickerViewModel.Hour = fiveMinutesFromCurrentTime.Hour;
            TimePickerViewModel.Minute = fiveMinutesFromCurrentTime.Minute;
        }
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedDateString))
        {
            IsSelectedDateStringValid = false;

            if (SelectedDateString.ConvertToDate(out DateTime selectedDate))
            {
                SelectedDate = selectedDate;
                IsSelectedDateStringValid = true;
            }
        }
        else if (e.PropertyName == nameof(SelectedDate))
        {
            SelectedDateString = SelectedDate.ConvertToString();
            IsSelectedDateStringValid = true;
        }
    }

    protected override void OnDispose()
    {
        if (!_closed)
        {
            PropertyChanged -= OnViewModelPropertyChanged;
        }
    }
}