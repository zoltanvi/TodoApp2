using System.Windows.Input;

namespace Modules.Common.DataBinding;

/// <summary>
/// A basic command that runs an action
/// </summary>
public class RelayParameterizedCommand<TParam> : ICommand
    where TParam : class
{
    /// <summary>
    /// The action to run
    /// </summary>
    private readonly Action<TParam> _action;

    /// <summary>
    /// The event that's fired when the <see cref="CanExecute(TParam)"/> value has changed
    /// </summary>
    public event EventHandler CanExecuteChanged;

    public RelayParameterizedCommand(Action<TParam> action)
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
        //IoC.UndoManager.ClearHistory();
        _action?.Invoke(parameter as TParam);
    }
}