namespace Modules.Common.Views.DragDrop;

public class DragDropEventArgs : EventArgs
{
    public int OldIndex { get; set; }
    public int NewIndex { get; set; }
    public object Item { get; set; }
}