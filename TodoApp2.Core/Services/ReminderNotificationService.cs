using System.Collections.Generic;
using System.Linq;

namespace TodoApp2.Core
{
    public class ReminderNotificationService
    {
        private const bool PlaySound = true;
        private bool m_CanShowNotification = true;
        private Queue<TaskListItemViewModel> m_NotificationQueue;

        private TaskScheduler TaskScheduler => IoC.ReminderTaskScheduler;

        public ReminderNotificationService()
        {
            m_NotificationQueue = new Queue<TaskListItemViewModel>();
            TaskScheduler.ScheduledAction = ShowNotification;

            Mediator.Instance.Register(OnNotificationClosed, ViewModelMessages.NotificationClosed);

            var taskList = IoC.ClientDatabase.GetActiveTaskItems();
            var filteredItems = new List<TaskListItemViewModel>(taskList.Where(task => task.IsReminderOn));

            foreach (var taskItem in filteredItems)
            {
                TaskScheduler.Schedule(taskItem, taskItem.ReminderDate);
            }
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
            m_NotificationQueue.Enqueue(task);

            OpenNextNotification();
        }

        private void OnNotificationClosed(object obj)
        {
            m_CanShowNotification = true;
            if (m_NotificationQueue.Count > 0)
            {
                OpenNextNotification();
            }
        }

        private void OpenNextNotification()
        {
            if (m_CanShowNotification)
            {
                m_CanShowNotification = false;

                TaskListItemViewModel notificationTask = m_NotificationQueue.Dequeue();
                notificationTask.IsReminderOn = false;
                IoC.ClientDatabase.UpdateTask(notificationTask);

                IoC.OverlayPageService.OpenPage(ApplicationPage.Notification, notificationTask);
                Mediator.Instance.NotifyClients(ViewModelMessages.WindowFlashRequested, PlaySound);
            }
        }
    }
}