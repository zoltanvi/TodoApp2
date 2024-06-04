using System.Windows.Input;

namespace Modules.Common.DataBinding;

/// <summary>
/// A basic command that runs an action
/// </summary>
public class RelayCommand : ICommand
{
    /// <summary>
    /// The action to run
    /// </summary>
    private readonly Action _action;

    /// <summary>
    /// The event that's fired when the <see cref="CanExecute(object)"/> value has changed
    /// </summary>
    public event EventHandler CanExecuteChanged;

    public RelayCommand(Action action)
    {
        _action = action;
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
        _action?.Invoke();
    }
}