using System;
using System.Windows.Input;

namespace TodoApp2.Core;

/// <summary>
/// A basic command that runs an action
/// </summary>
public class DynamicRelayCommand : ICommand
{
    /// <summary>
    /// The action to run
    /// </summary>
    public Action Action { get; set; }

    /// <summary>
    /// The event that's fired when the <see cref="CanExecute(object)"/> value has changed
    /// </summary>
    public event EventHandler CanExecuteChanged;

    public DynamicRelayCommand(Action action)
    {
        Action = action;
    }

    public DynamicRelayCommand()
    {
    }

    public void SetAction(Action action)
    {
        Action = action;
    }

    /// <summary>
    /// A relay command can always execute
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public bool CanExecute(object parameter) => true;

    /// <summary>
    /// Executes the command action
    /// </summary>
    /// <param name="parameter"></param>
    public void Execute(object parameter)
    {
        Action?.Invoke();
    }
}
