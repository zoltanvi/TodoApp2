using System;

namespace TodoApp2.Core;

public class UndoItem
{
    public CommandObject CommandObject { get; set; }

    public Func<CommandObject, CommandObject> RedoAction { get; }

    public Action<CommandObject> UndoAction { get; }

    public UndoItem(
        Func<CommandObject, CommandObject> redoAction,
        Action<CommandObject> undoAction,
        CommandObject commandObject)
    {
        RedoAction = redoAction;
        UndoAction = undoAction;
        CommandObject = commandObject;
    }
}
