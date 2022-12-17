using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using TodoApp2.Core;

namespace TodoApp2
{
    public class AsyncActionService : IAsyncActionService
    {
        private static IAsyncActionService m_Instance;
        private static List<DispatcherOperation> m_Operations = new List<DispatcherOperation>();

        public static IAsyncActionService Instance => m_Instance ?? (m_Instance = new AsyncActionService());

        private AsyncActionService() { }


        public void InvokeAsync(Action action)
        {
            DispatcherOperation currentOperation = Application.Current.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);

            m_Operations.Add(currentOperation);
        }

        public void AbortRunningActions()
        {
            foreach (DispatcherOperation operation in m_Operations)
            {
                operation.Abort();
            }

            m_Operations.Clear();
        }
    }
}
