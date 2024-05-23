using System;

namespace TodoApp2.Core;

public class UndoableCommand : IUndoableCommand
{
    private readonly UndoManager _undoManager;

    /// <summary>
    /// The function to run when executing the command.
    /// </summary>
    private readonly Func<CommandObject, CommandObject> _doAction;

    /// <summary>
    /// The function to run when redoing the command.
    /// </summary>
    private readonly Func<CommandObject, CommandObject> _redoAction;

    /// <summary>
    /// The action to run when undoing the command
    /// </summary>
    private readonly Action<CommandObject> _undoAction;

    /// <summary>
    /// The event that's fired when the <see cref="CanExecute(object)"/> value has changed
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Creates a new <see cref="UndoableCommand"/> instance.
    /// </summary>
    /// <param name="doAction">The callback to run when executing the command.</param>
    /// <param name="redoAction">The callback to run when redoing the command.</param>
    /// <param name="undoAction">The callback to run when undoing the command.</param>
    public UndoableCommand(
        Func<CommandObject, CommandObject> doAction,
        Func<CommandObject, CommandObject> redoAction,
        Action<CommandObject> undoAction)
    {
        _undoManager = IoC.UndoManager;
        _doAction = doAction;
        _redoAction = redoAction;
        _undoAction = undoAction;
    }

    /// <summary>
    /// A <see cref="UndoableCommand"/> can always execute.
    /// </summary>
    public bool CanExecute(object parameter) => true;

    /// <summary>
    /// Executes the command action
    /// </summary>
    public void Execute(object parameter)
    {
        _undoManager.Execute(_doAction, _redoAction, _undoAction, parameter);
    }
}
