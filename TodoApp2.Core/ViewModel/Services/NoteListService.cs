using System;
using System.Collections.ObjectModel;
using TodoApp2.Core.Extensions;
using TodoApp2.Core.Mappings;
using TodoApp2.Persistence;

namespace TodoApp2.Core;

public class NoteListService : BaseViewModel
{
    private AppViewModel _appViewModel;
    private IAppContext _context;
    private NoteViewModel _activeNote;

    public ObservableCollection<NoteViewModel> Items { get; set; }
    
    public NoteViewModel ActiveNote
    {
        get => _activeNote;
        set
        {
            SaveNoteContent();
            _activeNote = value;
            IoC.AppSettings.SessionSettings.ActiveNoteId = value?.Id ?? -1;
            Mediator.NotifyClients(ViewModelMessages.NoteChanged);
        }
    }

    public NoteListService(IAppContext context, AppViewModel appViewModel)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(appViewModel);

        _appViewModel = appViewModel;
        _context = context;

        var notes = _context.Notes
            .Where(x => !x.Trashed)
            .OrderByListOrder()
            .MapList();

        Items = new ObservableCollection<NoteViewModel>(notes);

        _activeNote = _context.Notes.First(x => x.Id == IoC.AppSettings.SessionSettings.ActiveNoteId).Map();
    }

    public void SaveNoteContent()
    {
        if (ActiveNote != null && _context.Notes.First(x => x.Id == ActiveNote.Id) != null)
        {
            _context.Notes.UpdateFirst(ActiveNote.Map());
        }
    }
}
