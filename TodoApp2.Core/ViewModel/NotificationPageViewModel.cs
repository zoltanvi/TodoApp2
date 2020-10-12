﻿using System.Windows.Input;

namespace TodoApp2.Core
{
    public class NotificationPageViewModel : BaseViewModel
    {
        /// <summary>
        /// The task to show the notification for.
        /// </summary>
        public TaskListItemViewModel NotificationTask { get; set; }

        /// <summary>
        /// Closes the notification page
        /// </summary>
        public ICommand CloseNotificationCommand { get; }

        public NotificationPageViewModel()
        {
            CloseNotificationCommand = new RelayCommand(CloseNotification);
        }

        public NotificationPageViewModel(TaskListItemViewModel notificationTask) : this()
        {
            NotificationTask = notificationTask;
            
            // If the notification shows up then the reminder has expired
            TurnOffReminder();
        }

        private void CloseNotification()
        {
            OverlayPageService.Instance.CloseNotificationPage();
        }

        private void TurnOffReminder()
        {
            // Turn off the reminder if the notification was closed with X button
            if (NotificationTask != null)
            {
                NotificationTask.IsReminderOn = false;
                IoC.ClientDatabase.UpdateTask(NotificationTask);
            }
        }
    }
}