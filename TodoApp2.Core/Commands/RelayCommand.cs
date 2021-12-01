﻿using System;
using System.Windows.Input;

namespace TodoApp2.Core
{
    /// <summary>
    /// A basic command that runs an action
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// The action to run
        /// </summary>
        private readonly Action m_Action;

        /// <summary>
        /// The event that's fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action action)
        {
            m_Action = action;
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
            m_Action?.Invoke();
        }
    }
}