using System;

namespace TodoApp2.Core
{
    public class CategoryChangedEventArgs : EventArgs
    {
        public CategoryListItemViewModel OriginalCategory { get; }
        public CategoryListItemViewModel ChangedCategory { get; }

        public CategoryChangedEventArgs(CategoryListItemViewModel original, CategoryListItemViewModel changed)
        {
            OriginalCategory = original;
            ChangedCategory = changed;
        }
    }
}
