using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace TodoApp2.Core
{
    public class ReminderTaskScheduler
    {
        private static readonly KeyValuePair<TaskListItemViewModel, DateTime> InvalidTask = 
            new KeyValuePair<TaskListItemViewModel, DateTime>(new TaskListItemViewModel(), DateTime.MinValue);

        private DispatcherTimer Timer { get; }
        private SortedList<TaskListItemViewModel, DateTime> ScheduledTasks { get; }
        private KeyValuePair<TaskListItemViewModel, DateTime> NextTask => ScheduledTasks.FirstOrDefault();
        private bool NextTaskExists => ScheduledTasks.Count > 0;
        private bool IsCurrentTaskValid => CurrentTask.Key != InvalidTask.Key && CurrentTask.Value != InvalidTask.Value;
        private KeyValuePair<TaskListItemViewModel, DateTime> CurrentTask { get; set; }

        /// <summary>
        /// The Action which is executed when the timer reaches the scheduled times.
        /// The action has one parameter.
        /// This parameter is the <see cref="TaskListItemViewModel"/>, the subject of the reminder.
        /// </summary>
        public Action<TaskListItemViewModel> ScheduledTask { get; set; }

        public ReminderTaskScheduler()
        {
            ScheduledTasks = new SortedList<TaskListItemViewModel, DateTime>();

            Timer = new DispatcherTimer { Interval = new TimeSpan(int.MaxValue) };
            Timer.Tick += TimerOnTick;

            MakeCurrentTaskInvalid();
        }

        public void Schedule(TaskListItemViewModel task, long dateTime)
        {
            Schedule(task, new DateTime(dateTime));
        }

        public void Schedule(TaskListItemViewModel task, DateTime dateTime)
        {
            // When the task to be set is the current task,
            // that means it is already started and waits to be executed.
            // In that case update the current task
            if (CurrentTask.Key == task)
            {
                InterruptCurrentTask();
                ScheduledTasks[task] = dateTime;
                StartNextTask();
            }
            // ... Or the task is scheduled but not started yet,
            // In that case update the scheduled task
            else if (ScheduledTasks.ContainsKey(task))
            {
                ScheduledTasks[task] = dateTime;
            }
            // ... Or the task is not scheduled yet, simply schedule it
            else
            {
                ScheduledTasks.Add(task, dateTime);
                InterruptCurrentTask();
                StartNextTask();
            }
        }

        public bool DeleteScheduled(TaskListItemViewModel task)
        {
            bool foundAndDeleted = false;

            // When the task to be deleted is the current task,
            // that means it is already started and waits to be executed.
            // In that case delete the current task and start the next valid one.
            if (CurrentTask.Key == task)
            {
                InterruptCurrentTask();
                ScheduledTasks.Remove(task);
                StartNextTask();
                foundAndDeleted = true;
            }
            // ... Or the task is scheduled but not started yet.
            // In that case simply delete from the list
            else if (ScheduledTasks.ContainsKey(task))
            {
                ScheduledTasks.Remove(task);
                foundAndDeleted = true;
            }

            return foundAndDeleted;
        }

        #region Private Methods

        /// <summary>
        /// Starts the next earliest task from the list.
        /// </summary>
        private void StartNextTask()
        {
            if (NextTaskExists)
            {
                bool anyTaskExecuted = false;
                DateTime current = DateTime.Now;

                // Delete from the list those tasks that are already expired
                // and execute the first task that is not expired yet
                for (int i = 0; i < ScheduledTasks.Count; i++)
                {
                    CurrentTask = NextTask;
                    ScheduledTasks.Remove(CurrentTask.Key);

                    TimeSpan executionTime = CurrentTask.Value - current;

                    // Execution time has not passed yet
                    if (executionTime >= TimeSpan.Zero)
                    {
                        ExecuteTask(CurrentTask);
                        anyTaskExecuted = true;
                        break;
                    }
                }

                if (!anyTaskExecuted)
                {
                    MakeCurrentTaskInvalid();
                }
            }
            else
            {
                StopTasks();
            }
        }

        /// <summary>
        /// Stops completely the task execution if there is any.
        /// </summary>
        private void StopTasks()
        {
            Timer.Stop();
            Timer.Interval = new TimeSpan(int.MaxValue);
            MakeCurrentTaskInvalid();
        }

        /// <summary>
        /// Stops the next scheduled task's execution and puts the task back
        /// to the list to be able to execute it in the future.
        /// </summary>
        private void InterruptCurrentTask()
        {
            if (IsCurrentTaskValid)
            {
                ScheduledTasks.Add(CurrentTask.Key, CurrentTask.Value);
                StopTasks();
            }
        }

        private void ExecuteTask(KeyValuePair<TaskListItemViewModel, DateTime> taskAndDate)
        {
            DateTime current = DateTime.Now;
            TimeSpan executionTime = taskAndDate.Value - current;

            // Execution time has not passed yet
            if (executionTime >= TimeSpan.Zero)
            {
                Timer.Interval = executionTime;
                Timer.Tag = taskAndDate;
                Timer.Start();
            }
            else
            {
                // The task won't be executed so there isn't any current task
                MakeCurrentTaskInvalid();
            }
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            if (sender is DispatcherTimer timer)
            {
                var todoTask = ((KeyValuePair<TaskListItemViewModel, DateTime>)timer.Tag).Key;

                StartNextTask();

                ScheduledTask?.Invoke(todoTask);

                MakeCurrentTaskInvalid();
            }
        }

        private void MakeCurrentTaskInvalid()
        {
            CurrentTask = InvalidTask;
        }

        #endregion Private Methods
    }
}