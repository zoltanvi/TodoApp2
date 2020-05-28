using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    public class ReminderTaskScheduler
    {
        private static readonly KeyValuePair<DateTime, int> InvalidTask = new KeyValuePair<DateTime, int>(DateTime.MinValue, int.MinValue);
        
        private readonly SortedList<DateTime, int> m_ScheduledTasks;

        private KeyValuePair<DateTime, int> m_CurrentTask;

        // TODO: Add another timer that checks if tasks are stuck in scheduledTasks.
        private Timer m_Timer;

        public bool NextTaskExists => m_ScheduledTasks.Count > 0;

        public Action<int> ScheduledTask { get; set; }

        public ReminderTaskScheduler()
        {
            m_ScheduledTasks = new SortedList<DateTime, int>();
            m_Timer = new Timer(Callback);
            m_CurrentTask = InvalidTask;
        }

        public void Schedule(long dateTime, int id)
        {
            Schedule(new DateTime(dateTime), id);
        }

        public void Schedule(DateTime dateTime, int id)
        {
            m_ScheduledTasks.Add(dateTime, id);
            InterruptCurrentTask();
            StartNextTask();
        }

        /// <summary>
        /// Starts the next earliest task from the list.
        /// </summary>
        private void StartNextTask()
        {
            if (NextTaskExists)
            {

                m_CurrentTask = m_ScheduledTasks.FirstOrDefault();
                m_ScheduledTasks.Remove(m_CurrentTask.Key);

                ExecuteTask(m_CurrentTask);
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
            m_Timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            m_CurrentTask = InvalidTask;
        }

        /// <summary>
        /// Stops the next scheduled task's execution and puts the task back
        /// to the list to be able to execute it in the future.
        /// </summary>
        private void InterruptCurrentTask()
        {
            if (m_CurrentTask.Key != InvalidTask.Key && m_CurrentTask.Value != InvalidTask.Value)
            { 
                m_ScheduledTasks.Add(m_CurrentTask.Key, m_CurrentTask.Value);
                StopTasks();
            }
        }

        private void ExecuteTask(KeyValuePair<DateTime, int> task)
        {
            DateTime current = DateTime.Now;
            TimeSpan executionTime = task.Key - current;
            
            // Execution time has not passed yet
            if (executionTime >= TimeSpan.Zero)
            {
                m_Timer.Dispose();
                m_Timer = new Timer(Callback);
                m_Timer.Change(executionTime, Timeout.InfiniteTimeSpan);
            }
            else
            {
                // The task won't be executed so there isn't any current task
                m_CurrentTask = InvalidTask;
            }
        }

        private void Callback(object state)
        {
            int todoTaskId = m_CurrentTask.Value;
            
            ScheduledTask?.Invoke(todoTaskId);

            m_CurrentTask = InvalidTask;

            StartNextTask();
        }
    }
}
