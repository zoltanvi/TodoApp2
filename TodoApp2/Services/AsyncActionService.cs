using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using TodoApp2.Core;

namespace TodoApp2
{
    public class AsyncActionService : IAsyncActionService
    {
        private static IAsyncActionService _instance;
        private static List<DispatcherOperation> _operations = new List<DispatcherOperation>();

        public static IAsyncActionService Instance => _instance ?? (_instance = new AsyncActionService());

        private AsyncActionService() { }


        public void InvokeAsync(Action action)
        {
            DispatcherOperation currentOperation = Application.Current.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);

            _operations.Add(currentOperation);
        }

        public void AbortRunningActions()
        {
            foreach (DispatcherOperation operation in _operations)
            {
                operation.Abort();
            }

            _operations.Clear();
        }
    }
}
