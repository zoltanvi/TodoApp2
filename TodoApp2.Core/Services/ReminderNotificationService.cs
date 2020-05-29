using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp2.Core.Services
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
                TaskScheduler.Schedule(taskItem.ReminderDate, taskItem.Id);
            }
        }

        private void ShowNotification(int taskId)
        {
            var task = Database.GetTask(taskId);
            IoC.Application.NotificationTask = task;

            Mediator.Instance.NotifyClients(ViewModelMessages.OpenNotificationPageRequested);

            // TODO: flash window here
        }
    }
}
