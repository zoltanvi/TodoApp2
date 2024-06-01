using Modules.Common.ViewModel;
using System;

namespace TodoApp2.Core;

public class CategoryViewModel : BaseViewModel, IReorderable, IEquatable<CategoryViewModel>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public long ListOrder { get; set; }
    public bool Trashed { get; set; }

    public bool Equals(CategoryViewModel other)
    {
        return other?.Id == Id;
    }
}