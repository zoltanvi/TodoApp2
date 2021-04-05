using System;

namespace TodoApp2.Core
{
    public class TaskChangedEventArgs : EventArgs
    {
        public TaskListItemViewModel Task { get; }

        public TaskChangedEventArgs(TaskListItemViewModel task)
        {
            Task = task;
        }
    }
}