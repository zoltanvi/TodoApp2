

using System;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class ReminderPageViewModel : BaseViewModel
    {
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
            if (ReminderTask != null)
            {
                IsReminderOn = ReminderTask.IsReminderOn;
                if (ReminderTask.ReminderDate != 0)
                {
                    var date = new DateTime(ReminderTask.ReminderDate);
                    SelectedDate = date.Date;
                    SelectedTime = new DateTime() + date.TimeOfDay;
                }
            }
            else
            {
                Console.WriteLine();
            }

            CloseReminderCommand = new RelayCommand(CloseReminder);
            SetReminderCommand = new RelayCommand(SetReminder);
        }

        private void SetReminder()
        {
            ReminderTask.IsReminderOn = IsReminderOn;
            ReminderTask.ReminderDate = (SelectedDate.Date + SelectedTime.TimeOfDay).Ticks;
            IoC.ReminderNotificationService.ReminderSet(ReminderTask);
            IoC.ClientDatabase.UpdateTask(ReminderTask);
        }

        private void CloseReminder()
        {
            Mediator.Instance.NotifyClients(ViewModelMessages.CloseReminderPageRequested);
        }
    }
}
