using Modules.Common.OBSOLETE.Mediator;
using Modules.Common.ViewModel;
using Modules.Notes.Repositories;
using Modules.Notes.ViewModels;
using Modules.Settings.Contracts.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TodoApp2.Core.Mappings;

namespace TodoApp2.Core;

public class NoteListService : BaseViewModel
{
    private AppViewModel _appViewModel;
    private INotesRepository _notesRepository;
    private NoteViewModel _activeNote;

    public ObservableCollection<NoteViewModel> Items { get; set; }
    
    public NoteViewModel ActiveNote
    {
        get => _activeNote;
        set
        {
            SaveNoteContent();
            _activeNote = value;
            AppSettings.Instance.SessionSettings.ActiveNoteId = value?.Id ?? -1;
            Mediator.NotifyClients(ViewModelMessages.NoteChanged);
        }
    }

    public NoteListService(INotesRepository notesRepository, AppViewModel appViewModel)
    {
        ArgumentNullException.ThrowIfNull(notesRepository);
        ArgumentNullException.ThrowIfNull(appViewModel);

        _appViewModel = appViewModel;
        _notesRepository = notesRepository;

        var notes = _notesRepository.GetActiveNotes();

        Items = new ObservableCollection<NoteViewModel>(notes.MapToViewModelList());

        _activeNote = Items.FirstOrDefault(x => x.Id == AppSettings.Instance.SessionSettings.ActiveNoteId);
    }

    public void SaveNoteContent()
    {
        if (ActiveNote != null)
        {
            _notesRepository.UpdateNoteContent(ActiveNote.Map());
        }
    }
}
