using System;

namespace TodoApp2.Core
{
    public class CategoryListItemViewModel : BaseViewModel, IReorderable, IEquatable<CategoryListItemViewModel>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long ListOrder { get; set; }
        public bool Trashed { get; set; }

        public bool Equals(CategoryListItemViewModel other)
        {
            return other?.Id == Id;
        }
    }
}