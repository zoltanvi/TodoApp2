using Modules.Common.ViewModel;
using PropertyChanged;

namespace Modules.Notes.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class NoteViewModel : BaseViewModel
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public int ListOrder { get; set; }
    public string? Content { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public bool IsDeleted { get; set; }
}
