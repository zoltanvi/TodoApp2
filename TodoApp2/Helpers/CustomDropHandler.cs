using System;
using System.Collections;
using System.Linq;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace TodoApp2
{
    public class DragDropEventArgs : EventArgs
    {
        public int OldIndex { get; set; }
        public int NewIndex { get; set; }
        public object Item { get; set; }
    }

    public class CustomDropHandler : DefaultDropHandler
    {
        public bool AlterInsertIndex { get; set; }
        public int NewInsertIndex { get; set; }

        public event EventHandler<DragDropEventArgs> BeforeItemDropped; 

        public override void Drop(IDropInfo dropInfo)
        {
            if (dropInfo?.DragInfo != null)
            {
                int insertIndex = dropInfo.UnfilteredInsertIndex;
                IList sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();
                object item = ExtractData(dropInfo.Data).OfType<object>().FirstOrDefault();
                int itemIndex = sourceList.IndexOf(item);

                if (itemIndex != -1)
                {
                    // Notify clients to allow them to alter the insert index.
                    var args = new DragDropEventArgs {Item = item, OldIndex = itemIndex, NewIndex = insertIndex};
                    BeforeItemDropped?.Invoke(this, args);

                    // Alter the insert index if the client requested it.
                    if (AlterInsertIndex)
                    {
                        insertIndex = NewInsertIndex;
                        AlterInsertIndex = false;
                    }

                    sourceList.Remove(item);

                    // The list got smaller, check for over-indexing (inserting into last index)
                    insertIndex = Math.Min(insertIndex, sourceList.Count);

                    sourceList.Insert(insertIndex, item);
                }
            }
        }
    }
}
