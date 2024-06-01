using Modules.Notes.Repositories.Models;

namespace Modules.Notes.ViewModels;

public static class NoteViewModelMappings
{
    public static Note Map(this NoteViewModel vm)
    {
        return new Note
        {
            Id = vm.Id,
            Title = vm.Title,
            Content = vm.Content,
            ListOrder = vm.ListOrder,
            CreationDate = vm.CreationDate,
            ModificationDate = vm.ModificationDate,
            IsDeleted = vm.IsDeleted,
        };
    }

    public static List<Note> MapList(this IEnumerable<NoteViewModel> vmList) =>
        vmList.Select(x => x.Map()).ToList();

    public static NoteViewModel MapToViewModel(this Note note)
    {
        return new NoteViewModel
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            ListOrder = note.ListOrder,
            CreationDate = note.CreationDate,
            ModificationDate = note.ModificationDate,
            IsDeleted = note.IsDeleted
        };
    }

    public static List<NoteViewModel> MapToViewModelList(this IEnumerable<Note> noteList) =>
        noteList.Select(x => x.MapToViewModel()).ToList();
}
