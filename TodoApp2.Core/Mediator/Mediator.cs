using System;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    public class AsyncEventArgs : EventArgs
    {
        public Func<Task> Func;
        public object Parameter;

        public AsyncEventArgs(Func<Task> func, object parameter)
        {
            Func = func;
            Parameter = parameter;
        }
    }

    public sealed class Mediator
    {
        private static Mediator m_Instance;
        public static Mediator Instance => m_Instance ?? (m_Instance = new Mediator());

        private readonly MultiDictionary<ViewModelMessages, Action<object>> m_MessageActionDictionary;
        private readonly MultiDictionary<ViewModelMessages, Func<Task>> m_MessageFunctionDictionary;
        private EventHandler<AsyncEventArgs> m_AsyncEvent;

        private Mediator()
        {
            m_MessageActionDictionary = new MultiDictionary<ViewModelMessages, Action<object>>();
            m_MessageFunctionDictionary = new MultiDictionary<ViewModelMessages, Func<Task>>();
            m_AsyncEvent += OnAsyncEvent;
        }

        private async void OnAsyncEvent(object sender, AsyncEventArgs e)
        {
            await e.Func();
        }

        /// <summary>
        /// Registers an action to a specific message.
        /// </summary>
        /// <param name="action">The action to use when the message is seen.</param>
        /// <param name="message">The message to register to.</param>
        public void Register(Action<object> action, ViewModelMessages message)
        {
            m_MessageActionDictionary.AddValue(message, action);
        }

        /// <summary>
        /// Registers a function to a specific message.
        /// </summary>
        /// <param name="action">The action to use when the message is seen.</param>
        /// <param name="message">The message to register to.</param>
        public void Register(Func<Task> task, ViewModelMessages message)
        {
            m_MessageFunctionDictionary.AddValue(message, task);
        }

        /// <summary>
        /// Notify all clients that are registered to the specific message
        /// </summary>
        /// <param name="message">The message for the notify by.</param>
        /// <param name="args">The arguments for the message.</param>
        public void NotifyClients(ViewModelMessages message, object args = null)
        {
            if (m_MessageActionDictionary.ContainsKey(message))
            {
                // forward the message to all listeners
                foreach (Action<object> action in m_MessageActionDictionary[message])
                {
                    action(args);
                }
            }

            if (m_MessageFunctionDictionary.ContainsKey(message))
            {
                // forward the message to all listeners
                foreach (var func in m_MessageFunctionDictionary[message])
                {
                    m_AsyncEvent.Invoke(this, new AsyncEventArgs(func, args));
                }
            }
        }
    }
}