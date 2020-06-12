using System;
using System.ComponentModel;
using System.Windows;

namespace TodoApp2.Core
{
    public class PropertyChangedWeakEventManager : WeakEventManager
    {
        private PropertyChangedWeakEventManager()
        {
        }

        /// <summary>
        /// Add a handler for the given source's event.
        /// </summary>
        public static void AddHandler(BaseViewModel source,
                                      EventHandler<PropertyChangedEventArgs> handler)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            CurrentManager.ProtectedAddHandler(source, handler);
        }

        /// <summary>
        /// Remove a handler for the given source's event.
        /// </summary>
        public static void RemoveHandler(BaseViewModel source,
                                         EventHandler<PropertyChangedEventArgs> handler)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            CurrentManager.ProtectedRemoveHandler(source, handler);
        }

        /// <summary>
        /// Get the event manager for the current thread.
        /// </summary>
        private static PropertyChangedWeakEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(PropertyChangedWeakEventManager);
                PropertyChangedWeakEventManager manager = (PropertyChangedWeakEventManager)GetCurrentManager(managerType);

                // at first use, create and register a new manager
                if (manager == null)
                {
                    manager = new PropertyChangedWeakEventManager();
                    SetCurrentManager(managerType, manager);
                }

                return manager;
            }
        }

        /// <summary>
        /// Return a new list to hold listeners to the event.
        /// </summary>
        protected override ListenerList NewListenerList()
        {
            return new ListenerList<PropertyChangedEventArgs>();
        }

        /// <summary>
        /// Listen to the given source for the event.
        /// </summary>
        protected override void StartListening(object source)
        {
            BaseViewModel typedSource = (BaseViewModel)source;
            typedSource.PropertyChanged += new PropertyChangedEventHandler(TypedSourceOnPropertyChanged);
        }

        /// <summary>
        /// Stop listening to the given source for the event.
        /// </summary>
        protected override void StopListening(object source)
        {
            BaseViewModel typedSource = (BaseViewModel)source;
            typedSource.PropertyChanged -= new PropertyChangedEventHandler(TypedSourceOnPropertyChanged);
        }

        /// <summary>
        /// Event handler for the PropertyChanged event.
        /// </summary>
        private void TypedSourceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DeliverEvent(sender, e);
        }

    }
}
