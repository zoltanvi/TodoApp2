using Modules.Common.OBSOLETE.Mediator;
using Modules.Common.ViewModel;
using Modules.Notes.Repositories;
using Modules.Notes.Views.Controls;
using Modules.Notes.Views.Mappings;
using Modules.Settings.Contracts.ViewModels;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Modules.Notes.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class NotePageViewModel : BaseViewModel
{
    private NoteViewModel? _activeNote;
    private readonly INotesRepository _notesRepository;
    private DispatcherTimer _timer;
    private bool _saved;
    private bool _initialized;

    public NotePageViewModel(INotesRepository notesRepository)
    {
        ArgumentNullException.ThrowIfNull(notesRepository);

        _notesRepository = notesRepository;
        _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 2) };
        _timer.Tick += TimerOnTick;

        var notes = notesRepository.GetActiveNotes();
        Items = new ObservableCollection<NoteViewModel>(notes.MapToViewModelList());
        _activeNote = Items.FirstOrDefault(x => x.Id == AppSettings.Instance.SessionSettings.ActiveNoteId);

        IsNoteExists = ActiveNote != null;

        MediatorOBSOLETE.Register(OnNoteChanged, ViewModelMessages.NoteChanged);
    }

    public ObservableCollection<NoteViewModel> Items { get; set; }

    public NoteViewModel? ActiveNote
    {
        get => _activeNote;
        set
        {
            SaveNoteContent();
            _activeNote = value;
            AppSettings.Instance.SessionSettings.ActiveNoteId = value?.Id ?? -1;
            MediatorOBSOLETE.NotifyClients(ViewModelMessages.NoteChanged);
        }
    }
    public bool IsNoteExists { get; private set; }

    private void OnNoteChanged(object obj)
    {
        IsNoteExists = ActiveNote != null;
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        if (!_saved)
        {
            SaveNoteContent();
        }
    }

    public void SaveNoteContent()
    {
        _saved = true;
        _timer.Stop();

        if (ActiveNote != null)
        {
            _notesRepository.UpdateNoteContent(ActiveNote.Map());
        }

        //_appViewModel.SaveIconVisible = !_appViewModel.SaveIconVisible;
    }

    public void NoteContentChanged()
    {
        if (_initialized)
        {
            _saved = false;
            _timer.Stop();
            _timer.Start();
        }
        else
        {
            _initialized = true;
        }
    }
}
