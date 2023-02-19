using System;

namespace TodoApp2.Core
{
    public class CategoryChangedEventArgs : EventArgs
    {
        public CategoryViewModel OriginalCategory { get; }
        public CategoryViewModel ChangedCategory { get; }

        public CategoryChangedEventArgs(CategoryViewModel original, CategoryViewModel changed)
        {
            OriginalCategory = original;
            ChangedCategory = changed;
        }
    }
}
