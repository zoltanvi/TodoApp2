﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TodoApp2.Core
{
    public class ReminderTaskScheduler
    {
        private static readonly KeyValuePair<DateTime, int> InvalidTask = new KeyValuePair<DateTime, int>(DateTime.MinValue, int.MinValue);
        
        private readonly SortedList<DateTime, int> m_ScheduledTasks;

        private KeyValuePair<DateTime, int> m_CurrentTask;

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
                DateTime current = DateTime.Now;

                // Delete from the list those tasks that are already expired
                // and execute the first task that is not expired yet
                for (int i = 0; i < m_ScheduledTasks.Count; i++)
                {
                    m_CurrentTask = m_ScheduledTasks.FirstOrDefault();
                    m_ScheduledTasks.Remove(m_CurrentTask.Key);

                    TimeSpan executionTime = m_CurrentTask.Key - current;
                    
                    // Execution time has not passed yet
                    if (executionTime >= TimeSpan.Zero)
                    {
                        ExecuteTask(m_CurrentTask);
                        break;
                    }
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
            // TODO: for some reason the ID is not right... Multithreading issue?
            int todoTaskId = m_CurrentTask.Value;
            
            // Start next task before executing the current one to prevent tasks
            StartNextTask();

            ScheduledTask?.Invoke(todoTaskId);

            m_CurrentTask = InvalidTask;
        }
    }
}