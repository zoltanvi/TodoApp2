using System;
using System.Diagnostics;

namespace TodoApp2.Core
{
    [DebuggerDisplay("Task ID [{ID}], Time [{DateTime}]")]
    public class ScheduleItem : IComparable<ScheduleItem>
    {
        public static readonly ScheduleItem Invalid = new ScheduleItem(-1, null, DateTime.MinValue);

        public int ID { get; private set; }
        public TaskListItemViewModel Task { get; }
        public DateTime DateTime { get; set; }

        public ScheduleItem(int id, TaskListItemViewModel task, DateTime dateTime)
        {
            ID = id;
            Task = task;
            DateTime = dateTime;
        }

        int IComparable<ScheduleItem>.CompareTo(ScheduleItem other)
        {
            return ID.CompareTo(other.ID);
        }
    }
}