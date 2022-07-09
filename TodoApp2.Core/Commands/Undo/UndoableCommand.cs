using System;

namespace TodoApp2.Core
{
    public class UndoableCommand : IUndoableCommand
    {
        private readonly UndoManager m_UndoManager;

        /// <summary>
        /// The function to run when executing the command.
        /// </summary>
        private readonly Func<CommandObject, CommandObject> m_DoAction;

        /// <summary>
        /// The function to run when redoing the command.
        /// </summary>
        private readonly Func<CommandObject, CommandObject> m_RedoAction;

        /// <summary>
        /// The action to run when undoing the command
        /// </summary>
        private readonly Action<CommandObject> m_UndoAction;

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
            m_UndoManager = IoC.UndoManager;
            m_DoAction = doAction;
            m_RedoAction = redoAction;
            m_UndoAction = undoAction;
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
            m_UndoManager.Execute(m_DoAction, m_RedoAction, m_UndoAction, parameter);
        }
    }
}
