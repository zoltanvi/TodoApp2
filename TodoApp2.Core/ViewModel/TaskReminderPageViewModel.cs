using System;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class TaskReminderPageViewModel : BaseViewModel
    {
        private ReminderNotificationService NotificationService => IoC.ReminderNotificationService;
        private OverlayPageService OverlayPageService => IoC.OverlayPageService;

        /// <summary>
        /// The task to show the notification for.
        /// </summary>
        public TaskListItemViewModel ReminderTask { get; }
        public long ReminderDateTime => ReminderTask?.ReminderDate ?? 0;
        public bool IsReminderOn { get; set; }

        public ICommand ClosePageCommand { get; }
        public ICommand EditReminderCommand { get; }
        public ICommand ChangeIsReminderOn { get; }

        public TaskReminderPageViewModel()
        {
            EditReminderCommand = new RelayCommand(EditReminder);
            ClosePageCommand = new RelayCommand(ClosePage);
            ChangeIsReminderOn = new RelayCommand(ChangeIsOn);
            OverlayPageService.SetBackgroundClickedAction(ClosePage);
        }

        private void ChangeIsOn()
        {
            ReminderTask.IsReminderOn = IsReminderOn;
            IoC.ClientDatabase.UpdateTask(ReminderTask);
        }

        public TaskReminderPageViewModel(TaskListItemViewModel reminderTask) : this()
        {
            if (reminderTask != null)
            {
                ReminderTask = reminderTask;
                IsReminderOn = ReminderTask.IsReminderOn;
            }
        }

        private void EditReminder()
        {
            IoC.OverlayPageService.OpenPage(ApplicationPage.ReminderEditor, ReminderTask);
        }

        private void SetReminder(bool isReminderOn)
        {
            IsReminderOn = isReminderOn;
            ReminderTask.IsReminderOn = IsReminderOn;
            IoC.ClientDatabase.UpdateTask(ReminderTask);

            NotificationService.SetReminder(ReminderTask);
        }

        private void ClosePage()
        {
            OverlayPageService.ClosePage();
        }
    }
}