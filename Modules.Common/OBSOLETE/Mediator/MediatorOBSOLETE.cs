namespace Modules.Common.OBSOLETE.Mediator;

public sealed class MediatorOBSOLETE
{
    private static MediatorOBSOLETE _instance;
    private static MediatorOBSOLETE Instance => _instance ?? (_instance = new MediatorOBSOLETE());

    private readonly MultiDictionary<ViewModelMessages, Action<object>> _messageActionDictionary;
    private readonly MultiDictionary<ViewModelMessages, Func<Task>> _messageFunctionDictionary;
    private readonly EventHandler<AsyncEventArgs> _asyncEvent;

    private MediatorOBSOLETE()
    {
        _messageActionDictionary = new MultiDictionary<ViewModelMessages, Action<object>>();
        _messageFunctionDictionary = new MultiDictionary<ViewModelMessages, Func<Task>>();
        _asyncEvent += OnAsyncEventAsync;
    }

    private async void OnAsyncEventAsync(object sender, AsyncEventArgs e)
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
        _messageActionDictionary.AddValue(message, action);
    }

    /// <inheritdoc cref="Register(Func{Task},ViewModelMessages)"/>
    private void InstanceRegister(Func<Task> task, ViewModelMessages message)
    {
        _messageFunctionDictionary.AddValue(message, task);
    }

    /// <inheritdoc cref="Deregister(Action{object},ViewModelMessages)"/>
    private void InstanceDeregister(Action<object> action, ViewModelMessages message)
    {
        _messageActionDictionary.RemoveValue(message, action);
    }

    /// <inheritdoc cref="Deregister(Func{Task},ViewModelMessages)"/>
    private void InstanceDeregister(Func<Task> task, ViewModelMessages message)
    {
        _messageFunctionDictionary.RemoveValue(message, task);
    }

    /// <inheritdoc cref="NotifyClients"/>
    private void InstanceNotifyClients(ViewModelMessages message, object args = null)
    {
        if (_messageActionDictionary.ContainsKey(message))
        {
            // forward the message to all listeners
            foreach (Action<object> action in _messageActionDictionary[message])
            {
                action(args);
            }
        }

        if (_messageFunctionDictionary.ContainsKey(message))
        {
            // forward the message to all listeners
            foreach (var func in _messageFunctionDictionary[message])
            {
                _asyncEvent.Invoke(this, new AsyncEventArgs(func, args));
            }
        }
    }
}