using System;

namespace TodoApp2.Core
{
    public class TaskChangedEventArgs : EventArgs
    {
        public TaskViewModel Task { get; }

        public TaskChangedEventArgs(TaskViewModel task)
        {
            Task = task;
        }
    }
}