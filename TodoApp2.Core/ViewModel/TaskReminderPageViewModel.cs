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
        public bool IsReminderOn
        {
            get => ReminderTask.IsReminderOn;
            set => ReminderTask.IsReminderOn = value;
        }

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
            // ReminderTask.IsReminderOn is modified with the toggle button,
            // so we persist the modification
            IoC.ClientDatabase.UpdateTask(ReminderTask);
        }

        public TaskReminderPageViewModel(TaskListItemViewModel reminderTask) : this()
        {
            if (reminderTask != null)
            {
                ReminderTask = IoC.ClientDatabase.GetTask(reminderTask.Id);
            }
        }

        private void EditReminder()
        {
            IoC.OverlayPageService.OpenPage(ApplicationPage.ReminderEditor, ReminderTask);
        }

        private void ClosePage()
        {
            OverlayPageService.ClosePage();
        }
    }
}