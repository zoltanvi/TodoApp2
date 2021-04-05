using System;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    public sealed class Mediator
    {
        private static Mediator s_Instance;
        private static Mediator Instance => s_Instance ?? (s_Instance = new Mediator());

        private readonly MultiDictionary<ViewModelMessages, Action<object>> m_MessageActionDictionary;
        private readonly MultiDictionary<ViewModelMessages, Func<Task>> m_MessageFunctionDictionary;
        private readonly EventHandler<AsyncEventArgs> m_AsyncEvent;

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
        public static void Register(Action<object> action, ViewModelMessages message)
        {
            Instance.InstanceRegister(action, message);
        }

        /// <summary>
        /// Registers a function to a specific message.
        /// </summary>
        /// <param name="task">The task to execute when the message is seen.</param>
        /// <param name="message">The message to register to.</param>
        public static void Register(Func<Task> task, ViewModelMessages message)
        {
            Instance.InstanceRegister(task, message);
        }

        /// <summary>
        /// Deregisters an action.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="message"></param>
        public static void Deregister(Action<object> action, ViewModelMessages message)
        {
            Instance.InstanceDeregister(action, message);
        }

        /// <summary>
        /// Deregisters a task.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="message"></param>
        public static void Deregister(Func<Task> task, ViewModelMessages message)
        {
            Instance.InstanceDeregister(task, message);
        }

        /// <summary>
        /// Notify all clients that are registered to the specific message
        /// </summary>
        /// <param name="message">The message for the notify by.</param>
        /// <param name="args">The arguments for the message.</param>
        public static void NotifyClients(ViewModelMessages message, object args = null)
        {
            Instance.InstanceNotifyClients(message, args);
        }

        /// <inheritdoc cref="Register(Action{object},ViewModelMessages)"/>
        private void InstanceRegister(Action<object> action, ViewModelMessages message)
        {
            m_MessageActionDictionary.AddValue(message, action);
        }

        /// <inheritdoc cref="Register(Func{Task},ViewModelMessages)"/>
        private void InstanceRegister(Func<Task> task, ViewModelMessages message)
        {
            m_MessageFunctionDictionary.AddValue(message, task);
        }

        /// <inheritdoc cref="Deregister(Action{object},ViewModelMessages)"/>
        private void InstanceDeregister(Action<object> action, ViewModelMessages message)
        {
            m_MessageActionDictionary.RemoveValue(message, action);
        }

        /// <inheritdoc cref="Deregister(Func{Task},ViewModelMessages)"/>
        private void InstanceDeregister(Func<Task> task, ViewModelMessages message)
        {
            m_MessageFunctionDictionary.RemoveValue(message, task);
        }

        /// <inheritdoc cref="NotifyClients"/>
        private void InstanceNotifyClients(ViewModelMessages message, object args = null)
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