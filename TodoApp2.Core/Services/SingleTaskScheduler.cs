using System;
using System.Collections.Generic;
using System.Threading;

namespace TodoApp2.Core
{
    public class SingleTaskScheduler
    {
        private readonly SortedSet<DateTime> m_ScheduledTimes;

        private DateTime m_CurrentTaskDate;

        private readonly Timer m_Timer;

        public bool NextTaskExists => m_ScheduledTimes.Count > 0;

        public Action ScheduledTask { get; set; }


        public SingleTaskScheduler()
        {
            m_ScheduledTimes = new SortedSet<DateTime>();
            m_Timer = new Timer(Callback);
            m_CurrentTaskDate = DateTime.MinValue;
        }

        public void Schedule(long dateTime)
        {
            Schedule(new DateTime(dateTime));
        }

        public void Schedule(DateTime dateTime)
        {
            m_ScheduledTimes.Add(dateTime);
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
                m_CurrentTaskDate = m_ScheduledTimes.Min;
                m_ScheduledTimes.Remove(m_CurrentTaskDate);

                ExecuteTask(m_CurrentTaskDate);
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
            m_CurrentTaskDate = DateTime.MinValue;
        }

        /// <summary>
        /// Stops the next scheduled task's execution and puts the task back
        /// to the list to be able to execute it in the future.
        /// </summary>
        private void InterruptCurrentTask()
        {
            if (m_CurrentTaskDate != DateTime.MinValue)
            { 
                m_ScheduledTimes.Add(m_CurrentTaskDate);
                StopTasks();
            }
        }

        private void ExecuteTask(DateTime dateTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan executionTime = dateTime - current;
            
            // Execution time has not passed yet
            if (executionTime >= TimeSpan.Zero)
            {
                m_Timer.Change(executionTime, Timeout.InfiniteTimeSpan);
            }
            else
            {
                // The task won't be executed so there isn't any current task
                m_CurrentTaskDate = DateTime.MinValue;
            }
        }

        private void Callback(object state)
        {
            ScheduledTask?.Invoke();

            m_CurrentTaskDate = DateTime.MinValue;

            StartNextTask();
        }
    }
}
