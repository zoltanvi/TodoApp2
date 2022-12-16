using System;
using System.Windows;
using System.Windows.Threading;
using TodoApp2.Core;

namespace TodoApp2
{
    public class AsyncActionService : IAsyncActionService
    {
        private static IAsyncActionService m_Instance;

        public static IAsyncActionService Instance => m_Instance ?? (m_Instance = new AsyncActionService());

        private AsyncActionService() { }

        public void InvokeAsync(Action action)
        {
            Application.Current.Dispatcher.Invoke(action, DispatcherPriority.Background);
        }
    }
}
