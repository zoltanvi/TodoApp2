using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace TodoApp2.Core;

public class TaskScheduler2
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);

    private DispatcherTimer _pollingTimer;
    private OrderedDictionary _taskDictionary;
    private Dictionary<TaskViewModel, DateTime> _taskDictionaryReversed;
    private int _nextId;
    private int NextId => _nextId++;

    /// <summary>
    /// The Action which is executed when the timer reaches the scheduled times.
    /// The action has one parameter.
    /// This parameter is the <see cref="TaskViewModel"/>, the subject of the reminder.
    /// </summary>
    public Action<TaskViewModel> ScheduledAction { get; set; }

    public TaskScheduler2()
    {
        _taskDictionary = new OrderedDictionary();
        _taskDictionaryReversed = new Dictionary<TaskViewModel, DateTime>();

        _pollingTimer = new DispatcherTimer { Interval = PollingInterval };
        _pollingTimer.Tick += OnPollingTimerTick;
    }

    public bool AddTask(TaskViewModel task, DateTime dateTime)
    {
        bool success = false;

        if (dateTime > DateTime.Now)
        {
            // Start polling when there is at least 1 item to watch
            if (_taskDictionary.Count == 0)
            {
                _pollingTimer.Start();
            }

            // Prevent tasks to be added with the same notification time
            while (_taskDictionary.Contains(dateTime))
            {
                dateTime = dateTime.AddSeconds(3);
            }

            ScheduleItem item = new ScheduleItem(NextId, task, dateTime);

            _taskDictionary.Add(dateTime, item);
            _taskDictionaryReversed.Add(task, dateTime);

            _pollingTimer.Tag = GetNearestScheduleItem();
            success = true;
        }

        return success;
    }

    internal bool ModifyTask(TaskViewModel task, DateTime reminderDate)
    {
        bool success = false;

        if (_taskDictionaryReversed.ContainsKey(task))
        {
            DateTime key = _taskDictionaryReversed[task];
            _taskDictionary.Remove(key);
            _taskDictionaryReversed.Remove(task);

            AddTask(task, reminderDate);

            success = true;
        }

        return success;
    }


    public bool DeleteTask(TaskViewModel task)
    {
        bool success = false;

        if (_taskDictionaryReversed.ContainsKey(task))
        {
            DateTime key = _taskDictionaryReversed[task];
            _taskDictionary.Remove(key);
            _taskDictionaryReversed.Remove(task);

            _pollingTimer.Tag = GetNearestScheduleItem();

            // Stop polling when there is no item to watch
            if (_taskDictionary.Count == 0)
            {
                _pollingTimer.Stop();
            }

            success = true;
        }

        return success;
    }

    private ScheduleItem GetNearestScheduleItem()
    {
        return _taskDictionary.Count == 0
            ? ScheduleItem.Invalid
            : _taskDictionary[0] as ScheduleItem;
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
