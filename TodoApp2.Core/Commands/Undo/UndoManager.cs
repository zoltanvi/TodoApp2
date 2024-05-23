using System;
using System.Collections.Generic;

namespace TodoApp2.Core;

public class UndoManager
{
    private readonly Stack<UndoItem> _undoStack;
    private readonly Stack<UndoItem> _redoStack;

    public UndoManager()
    {
        _undoStack = new Stack<UndoItem>();
        _redoStack = new Stack<UndoItem>();
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
            _undoStack.Push(undoItem);
            _redoStack.Clear();

            if (!string.IsNullOrEmpty(commandObject.UndoMessage))
            {
                IoC.MessageService.ShowUndo(commandObject.UndoMessage);
            }
        }
    }

    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            IoC.MessageService.HideMessage();

            UndoItem undoItem = _undoStack.Pop();
            undoItem.CommandObject.Handled = false;
            _redoStack.Push(undoItem);
            undoItem.UndoAction?.Invoke(undoItem.CommandObject);
            if (!string.IsNullOrEmpty(undoItem.CommandObject.UndoMessage))
            {
                IoC.MessageService.ShowInfo($"Undo: {undoItem.CommandObject.UndoMessage}");
            }
        }
    }

    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            UndoItem undoItem = _redoStack.Pop();
            undoItem.CommandObject.Handled = false;
            _undoStack.Push(undoItem);
            undoItem.RedoAction?.Invoke(undoItem.CommandObject);
            if (!string.IsNullOrEmpty(undoItem.CommandObject.UndoMessage))
            {
                IoC.MessageService.ShowUndo(undoItem.CommandObject.UndoMessage);
            }
        }
    }

    public void ClearHistory()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }
}
