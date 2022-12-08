using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp2.Core
{
    public class ReminderNotificationService
    {
        private const bool s_PlaySound = true;
        private bool m_CanShowNotification = true;
        private readonly Queue<TaskListItemViewModel> m_NotificationQueue;
        private readonly IDatabase m_Database;
        private readonly TaskScheduler2 m_TaskScheduler;
        private readonly OverlayPageService m_OverlayPageService;
        private Dictionary<TaskListItemViewModel, DateTime> m_ScheduledTasks;

        public ReminderNotificationService(IDatabase database, TaskScheduler2 taskScheduler, OverlayPageService overlayPageService)
        {
            m_Database = database;
            m_TaskScheduler = taskScheduler;
            m_OverlayPageService = overlayPageService;

            m_ScheduledTasks = new Dictionary<TaskListItemViewModel, DateTime>();
            m_NotificationQueue = new Queue<TaskListItemViewModel>();
            m_TaskScheduler.ScheduledAction = ShowNotification;

            Mediator.Register(OnNotificationClosed, ViewModelMessages.NotificationClosed);

            List<TaskListItemViewModel> taskList = m_Database.GetActiveTaskItemsWithReminder();

            foreach (TaskListItemViewModel task in taskList)
            {
                DateTime reminderDate = new DateTime(task.ReminderDate);

                if (m_TaskScheduler.AddTask(task, reminderDate))
                {
                    m_ScheduledTasks.Add(task, reminderDate);
                }
                else
                {
                    DateTime immediately = DateTime.Now.AddSeconds(3);
                    m_TaskScheduler.AddTask(task, immediately);
                }
            }
        }

        public void SetReminder(TaskListItemViewModel task)
        {
            DateTime reminderDate = new DateTime(task.ReminderDate);

            if (m_ScheduledTasks.ContainsKey(task))
            {
                if (!m_TaskScheduler.ModifyTask(task, reminderDate))
                {
                    m_ScheduledTasks.Remove(task);
                }
            }
            else
            {
                if (m_TaskScheduler.AddTask(task, reminderDate))
                {
                    m_ScheduledTasks.Add(task, reminderDate);
                }
                else
                {
                    task.IsReminderOn = false;
                    m_Database.UpdateTask(task);
                }
            }
        }

        public void DeleteReminder(TaskListItemViewModel task)
        {
            if (m_TaskScheduler.DeleteTask(task))
            {
                m_ScheduledTasks.Remove(task);
            }
        }

        private void ShowNotification(TaskListItemViewModel task)
        {
            m_ScheduledTasks.Remove(task);

            TaskListItemViewModel dbTask = m_Database.GetTask(task.Id);
            if (dbTask.IsReminderOn)
            {
                m_NotificationQueue.Enqueue(dbTask);
            }

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

                if (m_NotificationQueue.Count > 0)
                {
                    TaskListItemViewModel notificationTask = m_NotificationQueue.Dequeue();
                    notificationTask = m_Database.GetTask(notificationTask.Id);

                    if (notificationTask.Trashed)
                    {
                        OnNotificationClosed(null);
                    }

                    notificationTask.IsReminderOn = false;
                    m_Database.UpdateTask(notificationTask);

                    m_OverlayPageService.OpenPage(ApplicationPage.Notification, notificationTask);
                    Mediator.NotifyClients(ViewModelMessages.WindowFlashRequested, s_PlaySound);
                }
            }
        }
    }
}