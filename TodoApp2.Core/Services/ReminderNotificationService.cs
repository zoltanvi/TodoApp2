using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp2.Core
{
    public class ReminderNotificationService
    {
        const bool PlaySound = true;
        private bool m_CanShowNotification = true;
        private Queue<TaskListItemViewModel> m_NotificationQueue;

        private TaskScheduler TaskScheduler => IoC.ReminderTaskScheduler;
        private ClientDatabase Database => IoC.ClientDatabase;


        public ReminderNotificationService()
        {
            m_NotificationQueue = new Queue<TaskListItemViewModel>();
            TaskScheduler.ScheduledAction = ShowNotification;

            var taskList = Database.GetActiveTaskItems();
            var filteredItems = new List<TaskListItemViewModel>(taskList.Where(task => task.IsReminderOn));

            foreach (var taskItem in filteredItems)
            {
                TaskScheduler.Schedule(taskItem, taskItem.ReminderDate);
            }

            Mediator.Instance.Register(OnNotificationClosed, ViewModelMessages.OverlayBackgroundClosed);
        }

        public void SetReminder(TaskListItemViewModel task)
        {
            TaskScheduler.Schedule(task, task.ReminderDate);
        }

        public void DeleteReminder(TaskListItemViewModel task)
        {
            TaskScheduler.DeleteScheduled(task);
        }

        private void ShowNotification(TaskListItemViewModel task)
        {
            //m_NotificationQueue.Enqueue(task);

            //if (m_CanShowNotification)
            //{
            //    m_CanShowNotification = false;
                OpenNotification(task);
            //}
        }

        private void OnNotificationClosed(object obj)
        {
            //m_CanShowNotification = true;
            
            //if(m_NotificationQueue.Count > 0)
            //{
            //    OpenNotification(m_NotificationQueue.Dequeue());
            //}
        }

        private void OpenNotification(TaskListItemViewModel task)
        {
            OverlayPageService.Instance.OpenNotificationPage(task);

            Mediator.Instance.NotifyClients(ViewModelMessages.WindowFlashRequested, PlaySound);
        }
    }
}