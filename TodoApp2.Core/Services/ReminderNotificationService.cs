using System;
using System.Collections.Generic;

namespace TodoApp2.Core
{
    public class ReminderNotificationService
    {
        private const bool s_PlaySound = true;
        private bool m_CanShowNotification = true;
        private readonly Queue<TaskViewModel> m_NotificationQueue;
        private readonly IDatabase m_Database;
        private readonly TaskScheduler2 m_TaskScheduler;
        private readonly OverlayPageService m_OverlayPageService;
        private Dictionary<TaskViewModel, DateTime> m_ScheduledTasks;

        public ReminderNotificationService(IDatabase database, TaskScheduler2 taskScheduler, OverlayPageService overlayPageService)
        {
            m_Database = database;
            m_TaskScheduler = taskScheduler;
            m_OverlayPageService = overlayPageService;

            m_ScheduledTasks = new Dictionary<TaskViewModel, DateTime>();
            m_NotificationQueue = new Queue<TaskViewModel>();
            m_TaskScheduler.ScheduledAction = ShowNotification;

            Mediator.Register(OnNotificationClosed, ViewModelMessages.NotificationClosed);

            List<TaskViewModel> taskList = m_Database.GetActiveTasksWithReminder();

            foreach (TaskViewModel task in taskList)
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

        public void SetReminder(TaskViewModel task)
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

        public void DeleteReminder(TaskViewModel task)
        {
            if (m_TaskScheduler.DeleteTask(task))
            {
                m_ScheduledTasks.Remove(task);
            }
        }

        private void ShowNotification(TaskViewModel task)
        {
            m_ScheduledTasks.Remove(task);

            TaskViewModel dbTask = m_Database.GetTask(task.Id);
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
                    TaskViewModel notificationTask = m_NotificationQueue.Dequeue();
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