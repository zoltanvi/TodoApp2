using System;
using System.Collections.Generic;

namespace TodoApp2.Core
{
    public class UndoManager
    {
        private readonly Stack<UndoItem> m_UndoStack;
        private readonly Stack<UndoItem> m_RedoStack;

        public UndoManager()
        {
            m_UndoStack = new Stack<UndoItem>();
            m_RedoStack = new Stack<UndoItem>();
        }

        public void Execute(
            Func<CommandObject, CommandObject> doAction,
            Func<CommandObject, CommandObject> redoAction,
            Action<CommandObject> undoAction,
            object parameter = null)
        {
            CommandObject commandObject = doAction?.Invoke(new CommandObject(false, null, parameter));
            if (commandObject != null && commandObject.Handled)
            {
                UndoItem undoItem = new UndoItem(redoAction, undoAction, commandObject);
                m_UndoStack.Push(undoItem);
                m_RedoStack.Clear();

                if (!string.IsNullOrEmpty(commandObject.UndoMessage))
                {
                    IoC.MessageService.ShowUndo(commandObject.UndoMessage);
                }
            }
        }

        public void Undo()
        {
            if (m_UndoStack.Count > 0)
            {
                IoC.MessageService.HideMessage();

                UndoItem undoItem = m_UndoStack.Pop();
                undoItem.CommandObject.Handled = false;
                m_RedoStack.Push(undoItem);
                undoItem.UndoAction?.Invoke(undoItem.CommandObject);
            }
        }

        public void Redo()
        {
            if (m_RedoStack.Count > 0)
            {
                UndoItem undoItem = m_RedoStack.Pop();
                undoItem.CommandObject.Handled = false;
                m_UndoStack.Push(undoItem);
                undoItem.RedoAction?.Invoke(undoItem.CommandObject);
            }
        }

        public void ClearHistory()
        {
            m_UndoStack.Clear();
            m_RedoStack.Clear();
        }
    }
}
