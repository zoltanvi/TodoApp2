using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using System.Collections;

namespace Modules.Common.Views.DragDrop;

public class CustomDropHandler : DefaultDropHandler
{
    private static CustomDropHandler _instance = new CustomDropHandler();
    public static CustomDropHandler Instance => _instance;

    public bool AlterInsertIndex { get; set; }
    public int NewInsertIndex { get; set; }

    public event EventHandler<DragDropEventArgs> BeforeItemDropped;

    public override void Drop(IDropInfo dropInfo)
    {
        if (dropInfo?.DragInfo != null)
        {
            int newIndex = dropInfo.UnfilteredInsertIndex;
            IList sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();

            if (sourceList == null)
            {
                // Fixes the crash where the text is currently edited but somehow the drag & drop is active.
                return;
            }

            object item = ExtractData(dropInfo.Data).OfType<object>().FirstOrDefault();
            int oldIndex = sourceList.IndexOf(item);

            // Decrement index because the item is going to be removed before it is inserted again
            newIndex--;

            // Dropped above itself in the list, the previous decrementation was not necessary.
            if (newIndex + 1 <= oldIndex)
            {
                newIndex++;
            }

            if (oldIndex != -1)
            {
                // Notify clients to allow them to alter the insert index.
                var args = new DragDropEventArgs { Item = item, OldIndex = oldIndex, NewIndex = newIndex };
                BeforeItemDropped?.Invoke(this, args);

                // Alter the insert index if the client requested it.
                if (AlterInsertIndex)
                {
                    newIndex = NewInsertIndex;
                    AlterInsertIndex = false;
                }

                sourceList.Remove(item);

                // The list got smaller, check for over-indexing (inserting into last index)
                newIndex = Math.Min(newIndex, sourceList.Count);

                sourceList.Insert(newIndex, item);
            }
        }
    }
}