using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp2.Core
{
    public class ReminderNotificationService
    {
        private ReminderTaskScheduler TaskScheduler => IoC.ReminderTaskScheduler;

        private ClientDatabase Database => IoC.ClientDatabase;

        public ReminderNotificationService()
        {
            TaskScheduler.ScheduledTask = ShowNotification;

            var taskList = Database.GetActiveTaskItems();
            var filteredItems = new List<TaskListItemViewModel>(taskList.Where(task => task.IsReminderOn));

            foreach (var taskItem in filteredItems)
            {
                TaskScheduler.Schedule(taskItem.Id, taskItem.ReminderDate);
            }

            #region TEST

            var now = DateTime.Now;
            //now = now.AddSeconds(100);

            //for (int i = 0; i < 10; i++)
            //{
            //    now = now.AddSeconds(-10);
            //    TaskScheduler.Schedule(now, i);
            //}

            //TaskScheduler.Schedule(now.AddSeconds(10), 17);

            #endregion TEST
        }

        public void SetReminder(TaskListItemViewModel task)
        {
            TaskScheduler.Schedule(task.Id, task.ReminderDate);
        }

        public void DeleteReminder(TaskListItemViewModel task)
        {
            TaskScheduler.DeleteScheduled(task.Id);
        }

        private void ShowNotification(int taskId)
        {
            const bool playSound = true;

            // Query the task before showing the notification because
            // the task description can change between
            // scheduling and executing to show the task
            TaskListItemViewModel task = Database.GetTask(taskId);

            Mediator.Instance.NotifyClients(ViewModelMessages.OpenNotificationPageRequested, task);

            Mediator.Instance.NotifyClients(ViewModelMessages.WindowFlashRequested, playSound);
        }
    }
}