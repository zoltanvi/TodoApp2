using System;

namespace TodoApp2.Core
{
    public sealed class Mediator
    {
        private static Mediator m_Instance;
        public static Mediator Instance => m_Instance ?? (m_Instance = new Mediator());

        private readonly MultiDictionary<ViewModelMessages, Action<object>> m_MessageActionDictionary;

        private Mediator()
        {
            m_MessageActionDictionary = new MultiDictionary<ViewModelMessages, Action<object>>();
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
        /// Notify all clients that are registered to the specific message
        /// </summary>
        /// <param name="message">The message for the notify by.</param>
        /// <param name="args">The arguments for the message.</param>
        public void NotifyClients(ViewModelMessages message, object args)
        {
            if (m_MessageActionDictionary.ContainsKey(message))
            {
                // forward the message to all listeners
                foreach (Action<object> action in m_MessageActionDictionary[message])
                {
                    action(args);
                }
            }
        }
    }
}
