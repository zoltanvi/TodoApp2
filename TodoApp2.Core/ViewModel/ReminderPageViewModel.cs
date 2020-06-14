

using System;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class ReminderPageViewModel : BaseViewModel
    {
        private ReminderNotificationService NotificationService => IoC.ReminderNotificationService;
        private ClientDatabase ClientDatabase => IoC.ClientDatabase;

        /// <summary>
        /// The task to show the notification for.
        /// </summary>
        public TaskListItemViewModel ReminderTask { get; set; }

        public DateTime SelectedDate { get; set; } = DateTime.Now;
        
        public DateTime SelectedTime { get; set; } = DateTime.Now;

        public bool IsReminderOn { get; set; }

        public ICommand CloseReminderCommand { get; }

        public ICommand SetReminderCommand { get; }

        public ReminderPageViewModel()
        {
            CloseReminderCommand = new RelayCommand(CloseReminder);
            SetReminderCommand = new RelayCommand(SetReminder);
        }


        public ReminderPageViewModel(TaskListItemViewModel reminderTask) : this()
        {
            if (reminderTask != null)
            {
                ReminderTask = reminderTask;
                IsReminderOn = ReminderTask.IsReminderOn;
                if (ReminderTask.ReminderDate != 0)
                {
                    DateTime date = new DateTime(ReminderTask.ReminderDate);
                    SelectedDate = date.Date;
                    SelectedTime = new DateTime() + date.TimeOfDay;
                }
            }
        }

        private void SetReminder()
        {
            UpdateTaskReminder();

            if (IsReminderOn)
            {
                NotificationService.SetReminder(ReminderTask);
            }
            else
            {
                NotificationService.DeleteReminder(ReminderTask);
            }
            
            CloseReminder();
        }

        private void UpdateTaskReminder()
        {
            DateTime reminderDate = SelectedDate.Date + new TimeSpan(SelectedTime.Hour, SelectedTime.Minute, 0);
            ReminderTask.ReminderDate = reminderDate.Ticks;

            ReminderTask.IsReminderOn = IsReminderOn;
            ClientDatabase.UpdateTask(ReminderTask);
        }

        private void CloseReminder()
        {
            Mediator.Instance.NotifyClients(ViewModelMessages.CloseReminderPageRequested);
        }
    }
}
