using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace TodoApp2.Core
{
    public class TaskScheduler2
    {
        private static readonly TimeSpan s_PollingInterval = TimeSpan.FromSeconds(5);

        private DispatcherTimer m_PollingTimer;
        private OrderedDictionary m_TaskDictionary;
        private Dictionary<TaskViewModel, DateTime> m_TaskDictionaryReversed;
        private int m_NextId = 0;
        private int NextId => m_NextId++;

        /// <summary>
        /// The Action which is executed when the timer reaches the scheduled times.
        /// The action has one parameter.
        /// This parameter is the <see cref="TaskViewModel"/>, the subject of the reminder.
        /// </summary>
        public Action<TaskViewModel> ScheduledAction { get; set; }

        public TaskScheduler2()
        {
            m_TaskDictionary = new OrderedDictionary();
            m_TaskDictionaryReversed = new Dictionary<TaskViewModel, DateTime>();

            m_PollingTimer = new DispatcherTimer { Interval = s_PollingInterval };
            m_PollingTimer.Tick += OnPollingTimerTick;
        }

        public bool AddTask(TaskViewModel task, DateTime dateTime)
        {
            bool success = false;

            // If the task has already expired, ignore it
            if (dateTime > DateTime.Now)
            {
                // Start polling when there is at least 1 item to watch
                if (m_TaskDictionary.Count == 0)
                {
                    m_PollingTimer.Start();
                }

                // Prevent tasks to be added with the same notification time
                while (m_TaskDictionary.Contains(dateTime))
                {
                    dateTime = dateTime.AddSeconds(3);
                }

                ScheduleItem item = new ScheduleItem(NextId, task, dateTime);

                m_TaskDictionary.Add(dateTime, item);
                m_TaskDictionaryReversed.Add(task, dateTime);

                m_PollingTimer.Tag = GetNearestScheduleItem();
                success = true;
            }

            return success;
        }

        internal bool ModifyTask(TaskViewModel task, DateTime reminderDate)
        {
            bool success = false;

            if (m_TaskDictionaryReversed.ContainsKey(task))
            {
                DateTime key = m_TaskDictionaryReversed[task];
                m_TaskDictionary.Remove(key);
                m_TaskDictionaryReversed.Remove(task);

                AddTask(task, reminderDate);

                success = true;
            }

            return success;
        }


        public bool DeleteTask(TaskViewModel task)
        {
            bool success = false;

            if (m_TaskDictionaryReversed.ContainsKey(task))
            {
                DateTime key = m_TaskDictionaryReversed[task];
                m_TaskDictionary.Remove(key);
                m_TaskDictionaryReversed.Remove(task);

                m_PollingTimer.Tag = GetNearestScheduleItem();

                // Stop polling when there is no item to watch
                if (m_TaskDictionary.Count == 0)
                {
                    m_PollingTimer.Stop();
                }

                success = true;
            }

            return success;
        }

        private ScheduleItem GetNearestScheduleItem()
        {
            return m_TaskDictionary.Count == 0
                ? ScheduleItem.Invalid
                : m_TaskDictionary[0] as ScheduleItem;
        }

        private void OnPollingTimerTick(object sender, EventArgs e)
        {
            if (sender is DispatcherTimer dispatcherTimer)
            {
                ScheduleItem nearestScheduleItem = GetNearestScheduleItem();

                if (nearestScheduleItem != ScheduleItem.Invalid &&
                    nearestScheduleItem.DateTime <= DateTime.Now)
                {
                    ScheduledAction?.Invoke(nearestScheduleItem.Task);
                    DeleteTask(nearestScheduleItem.Task);
                }
            }
        }
    }
}
