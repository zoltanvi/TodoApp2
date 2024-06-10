using Modules.Common.ViewModel;
using PropertyChanged;

namespace Modules.Categories.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class CategoryViewModel : BaseViewModel, IEquatable<CategoryViewModel>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int ListOrder { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public bool IsDeleted { get; set; }

    public bool Equals(CategoryViewModel? other)
    {
        return other != null && other.Id == Id;
    }
}