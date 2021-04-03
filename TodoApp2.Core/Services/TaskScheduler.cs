using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;

namespace TodoApp2.Core
{
    [DebuggerDisplay("Task [{Task.Content}], Time [{DateTime}]")]
    public class ScheduleItem : IComparable<ScheduleItem>
    {
        public static readonly ScheduleItem Invalid = new ScheduleItem(null, DateTime.MinValue);

        public TaskListItemViewModel Task { get; }
        public DateTime DateTime { get; set; }

        public ScheduleItem(TaskListItemViewModel task, DateTime dateTime)
        {
            Task = task;
            DateTime = dateTime;
        }

        int IComparable<ScheduleItem>.CompareTo(ScheduleItem other)
        {
            return DateTime.CompareTo(other.DateTime);
        }
    }

    public class TaskScheduler
    {
        private DispatcherTimer Timer { get; }
        private List<ScheduleItem> ScheduledItems { get; }
        private ScheduleItem NextItemToSchedule => ScheduledItems.Min();
        private ScheduleItem CurrentItem { get; set; }
        private bool NextItemExists => ScheduledItems.Count > 0;
        private bool IsCurrentItemValid => !ReferenceEquals(CurrentItem, ScheduleItem.Invalid);

        /// <summary>
        /// The Action which is executed when the timer reaches the scheduled times.
        /// The action has one parameter.
        /// This parameter is the <see cref="TaskListItemViewModel"/>, the subject of the reminder.
        /// </summary>
        public Action<TaskListItemViewModel> ScheduledAction { get; set; }

        public TaskScheduler()
        {
            ScheduledItems = new List<ScheduleItem>();

            Timer = new DispatcherTimer { Interval = new TimeSpan(int.MaxValue) };
            Timer.Tick += TimerOnTick;

            CurrentItem = ScheduleItem.Invalid;
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
            if (CurrentItem.Task != null && CurrentItem.Task.Equals(task))
            {
                // Stop execution
                InterruptCurrentTask();

                // Get the existing item from the list
                GetScheduledItem(task, out ScheduleItem scheduledItem);

                // Update the execution time
                scheduledItem.DateTime = dateTime;

                // Continue execution
                StartNextTask();
            }
            // ... Or the task is scheduled but not started yet,
            // in that case update the scheduled task
            else if (GetScheduledItem(task, out ScheduleItem scheduledItem))
            {
                scheduledItem.DateTime = dateTime;
            }
            // ... Or the task is not scheduled yet, simply schedule it
            else
            {
                while (ScheduledItems.Any(i => i.DateTime == dateTime) || CurrentItem.DateTime == dateTime)
                {
                    // Prevent items with same dateTime, add 5 seconds
                    // This is needed for the notifications to work properly
                    dateTime += new TimeSpan(0, 0, 0, 5);
                }

                ScheduledItems.Add(new ScheduleItem(task, dateTime));
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
            if (CurrentItem.Task != null && CurrentItem.Task.Equals(task))
            {
                InterruptCurrentTask();
                RemoveScheduledItem(task);
                StartNextTask();
                foundAndDeleted = true;
            }
            // ... Or the task is scheduled but not started yet.
            // In that case simply delete from the list
            else if (GetScheduledItem(task, out _))
            {
                RemoveScheduledItem(task);
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
            if (NextItemExists)
            {
                CurrentItem = NextItemToSchedule;
                ScheduledItems.Remove(CurrentItem);

                ExecuteTask(CurrentItem);
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
            CurrentItem = ScheduleItem.Invalid;
        }

        /// <summary>
        /// Stops the next scheduled task's execution and puts the task back
        /// to the list to be able to execute it in the future.
        /// </summary>
        private void InterruptCurrentTask()
        {
            if (IsCurrentItemValid)
            {
                ScheduledItems.Add(CurrentItem);
                StopTasks();
            }
        }

        private void ExecuteTask(ScheduleItem scheduleItem)
        {
            TimeSpan executionTime = scheduleItem.DateTime - DateTime.Now;

            if (executionTime < TimeSpan.Zero)
            {
                executionTime = new TimeSpan(10);
            }

            Timer.Interval = executionTime;

            // Set the timer tag to the scheduled item so 
            // it can be accessed when the timer ticks
            Timer.Tag = scheduleItem;
            Timer.Start();
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            if (sender is DispatcherTimer timer)
            {
                TaskListItemViewModel task = ((ScheduleItem)timer.Tag).Task;

                StartNextTask();

                ScheduledAction?.Invoke(task);

                CurrentItem = ScheduleItem.Invalid;
            }
        }

        /// <summary>
        /// Finds the <see cref="ScheduleItem"/> with the <paramref name="task"/> from <see cref="ScheduledItems"/>.
        /// If the item was not found, the <paramref name="foundItem"/> is <see cref="ScheduleItem.Invalid"/>.
        /// </summary>
        /// <param name="task">The <see cref="TaskListItemViewModel"/> to find.</param>
        /// <param name="foundItem">The found <see cref="ScheduleItem"/>.</param>
        /// <returns>Returns true if the <see cref="ScheduleItem"/> is found, false otherwise.</returns>
        private bool GetScheduledItem(TaskListItemViewModel task, out ScheduleItem foundItem)
        {
            bool found = true;
            foundItem = ScheduledItems.FirstOrDefault(i => i.Task.Equals(task));

            if (foundItem == null)
            {
                found = false;
                foundItem = ScheduleItem.Invalid;
            }

            return found;
        }

        private void RemoveScheduledItem(TaskListItemViewModel task)
        {
            GetScheduledItem(task, out ScheduleItem item);
            ScheduledItems.Remove(item);
        }

        #endregion Private Methods
    }
}