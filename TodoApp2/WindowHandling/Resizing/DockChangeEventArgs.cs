using System;

namespace TodoApp2.WindowHandling.Resizing;

public class DockChangeEventArgs : EventArgs
{
    public DockChangeEventArgs(bool isDocked)
    {
        IsDocked = isDocked;
    }

    public bool IsDocked { get; }
}