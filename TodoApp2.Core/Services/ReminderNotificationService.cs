using System;
using System.Collections.Generic;
using TodoApp2.Common;
using TodoApp2.Core.Mappings;
using TodoApp2.Persistence;

namespace TodoApp2.Core
{
    public class ReminderNotificationService
    {
        private const bool PlaySound = true;
        private bool _canShowNotification = true;
        private readonly Queue<TaskViewModel> _notificationQueue;
        private readonly IAppContext _context;
        private readonly TaskScheduler2 _taskScheduler;
        private readonly TaskListService _taskListService;
        private Dictionary<TaskViewModel, DateTime> _scheduledTasks;

        public ReminderNotificationService(
            IAppContext context, 
            TaskScheduler2 taskScheduler, 
            TaskListService taskListService)
        {
            ThrowHelper.ThrowIfNull(context);
            ThrowHelper.ThrowIfNull(taskScheduler);
            ThrowHelper.ThrowIfNull(taskListService);

            _context = context;
            _taskScheduler = taskScheduler;
            _taskListService = taskListService;

            _scheduledTasks = new Dictionary<TaskViewModel, DateTime>();
            _notificationQueue = new Queue<TaskViewModel>();
            _taskScheduler.ScheduledAction = ShowNotification;

            Mediator.Register(OnNotificationClosed, ViewModelMessages.NotificationClosed);

            var taskList = _context.Tasks
                .Where(x => x.IsReminderOn && !x.Trashed && x.ReminderDate != 0)
                .MapList();

            foreach (TaskViewModel task in taskList)
            {
                DateTime reminderDate = new DateTime(task.ReminderDate);

                if (_taskScheduler.AddTask(task, reminderDate))
                {
                    _scheduledTasks.Add(task, reminderDate);
                }
                else
                {
                    DateTime immediately = DateTime.Now.AddSeconds(3);
                    _taskScheduler.AddTask(task, immediately);
                }
            }
        }

        public void SetReminder(TaskViewModel task)
        {
            DateTime reminderDate = new DateTime(task.ReminderDate);

            if (_scheduledTasks.ContainsKey(task))
            {
                if (!_taskScheduler.ModifyTask(task, reminderDate))
                {
                    _scheduledTasks.Remove(task);
                }
            }
            else
            {
                if (_taskScheduler.AddTask(task, reminderDate))
                {
                    _scheduledTasks.Add(task, reminderDate);
                }
                else
                {
                    task.IsReminderOn = false;

                    _taskListService.UpdateTask(task);
                }
            }
        }

        public void DeleteReminder(TaskViewModel task)
        {
            if (_taskScheduler.DeleteTask(task))
            {
                _scheduledTasks.Remove(task);
            }
        }

        private void ShowNotification(TaskViewModel task)
        {
            _scheduledTasks.Remove(task);

            if (task.IsReminderOn)
            {
                _notificationQueue.Enqueue(task);
            }

            OpenNextNotification();
        }

        private void OnNotificationClosed(object obj)
        {
            _canShowNotification = true;
            if (_notificationQueue.Count > 0)
            {
                OpenNextNotification();
            }
        }

        private void OpenNextNotification()
        {
            if (_canShowNotification)
            {
                _canShowNotification = false;

                if (_notificationQueue.Count > 0)
                {
                    // Always get the task from the database here,
                    // because it may be inconsistent with the queued instance
                    TaskViewModel notificationTask = _notificationQueue.Dequeue();
                    notificationTask = _context.Tasks.First(x => x.Id == notificationTask.Id).Map();

                    if (notificationTask.Trashed)
                    {
                        OnNotificationClosed(null);
                    }

                    notificationTask.IsReminderOn = false;
                    _taskListService.UpdateTask(notificationTask);

                    IoC.AppViewModel.OpenPage(ApplicationPage.Notification, notificationTask);
                    Mediator.NotifyClients(ViewModelMessages.WindowFlashRequested, PlaySound);
                }
            }
        }
    }
}