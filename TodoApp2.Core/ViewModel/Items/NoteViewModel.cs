using System;

namespace TodoApp2.Core;

public class NoteViewModel : BaseViewModel, IReorderable, IEquatable<NoteViewModel>
{
    public int Id { get; set; }
    public string Title { get; set; }
    public long ListOrder { get; set; }
    public string Content { get; set; }
    public long CreationDate { get; set; }
    public long ModificationDate { get; set; }
    public bool Trashed { get; set; }

    public bool Equals(NoteViewModel other)
    {
        return other?.Id == Id;
    }
}
