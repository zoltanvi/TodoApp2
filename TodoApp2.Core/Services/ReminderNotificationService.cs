using System.Collections.Generic;
using System.Linq;

namespace TodoApp2.Core
{
    public class ReminderNotificationService
    {
        private const bool s_PlaySound = true;
        private bool m_CanShowNotification = true;
        private readonly Queue<TaskListItemViewModel> m_NotificationQueue;
        private readonly TaskScheduler m_TaskScheduler;
        private readonly ClientDatabase m_ClientDatabase;

        public ReminderNotificationService(TaskScheduler taskScheduler, ClientDatabase clientDatabase)
        {
            m_TaskScheduler = taskScheduler;
            m_ClientDatabase = clientDatabase;

            m_NotificationQueue = new Queue<TaskListItemViewModel>();
            m_TaskScheduler.ScheduledAction = ShowNotification;

            Mediator.Register(OnNotificationClosed, ViewModelMessages.NotificationClosed);

            List<TaskListItemViewModel> taskList = m_ClientDatabase.GetActiveTaskItems();
            List<TaskListItemViewModel> filteredItems = new List<TaskListItemViewModel>(taskList.Where(task => task.IsReminderOn));

            foreach (var taskItem in filteredItems)
            {
                m_TaskScheduler.Schedule(taskItem, taskItem.ReminderDate);
            }
        }

        public void SetReminder(TaskListItemViewModel task)
        {
            m_TaskScheduler.Schedule(task, task.ReminderDate);
        }

        public void DeleteReminder(TaskListItemViewModel task)
        {
            m_TaskScheduler.DeleteScheduled(task);
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

                while (m_NotificationQueue.Count > 0)
                {
                    TaskListItemViewModel notificationTask = m_NotificationQueue.Dequeue();
                    notificationTask = m_ClientDatabase.GetTask(notificationTask.Id);

                    if (notificationTask.Trashed)
                    {
                        OnNotificationClosed(null);
                        break;
                    }

                    notificationTask.IsReminderOn = false;
                    m_ClientDatabase.UpdateTask(notificationTask);

                    IoC.OverlayPageService.OpenPage(ApplicationPage.Notification, notificationTask);
                    Mediator.NotifyClients(ViewModelMessages.WindowFlashRequested, s_PlaySound);
                }
            }
        }
    }
}