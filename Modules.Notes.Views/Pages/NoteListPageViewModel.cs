using MediatR;
using Modules.Common;
using Modules.Common.DataBinding;
using Modules.Common.Navigation;
using Modules.Common.OBSOLETE.Mediator;
using Modules.Common.Services.Navigation;
using Modules.Common.ViewModel;
using Modules.Common.Views.Services.Navigation;
using Modules.Notes.Repositories;
using Modules.Notes.Repositories.Models;
using Modules.Notes.Views.Controls;
using Modules.Notes.Views.Mappings;
using Modules.Settings.Contracts.ViewModels;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Modules.Notes.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class NoteListPageViewModel : BaseViewModel
{
    private readonly INotesRepository _notesRepository;
    private readonly IMediator _mediator;
    private NoteViewModel? _activeNote;
    private readonly IMainPageNavigationService _mainPageNavigationService;
    private readonly ISideMenuPageNavigationService _sideMenuPageNavigationService;

    private int _lastRemovedId = int.MinValue;

    public NoteListPageViewModel(
        INotesRepository notesRepository,
        IMainPageNavigationService mainPageNavigationService,
        ISideMenuPageNavigationService sideMenuPageNavigationService,
        IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(notesRepository);
        ArgumentNullException.ThrowIfNull(mediator);

        _notesRepository = notesRepository;
        _mediator = mediator;
        _mainPageNavigationService = mainPageNavigationService;
        _sideMenuPageNavigationService = sideMenuPageNavigationService;

        AddNoteCommand = new RelayCommand(AddNote);
        DeleteNoteCommand = new RelayParameterizedCommand<NoteViewModel>(DeleteNote);
        OpenNoteCommand = new RelayParameterizedCommand<NoteViewModel>(SetActiveNote);
        OpenSettingsPageCommand = new RelayCommand(OpenSettingsPage);
        OpenCategoryPageCommand = new RelayCommand(OpenCategoryPage);

        var notes = _notesRepository.GetActiveNotes();
        Items = new ObservableCollection<NoteViewModel>(notes.MapToViewModelList());

        // Load the application settings to update the ActiveNote
        MediatorOBSOLETE.NotifyClients(ViewModelMessages.LoadAppSettings);

        _activeNote = Items.FirstOrDefault(x => x.Id == AppSettings.Instance.SessionSettings.ActiveNoteId);

        Items.CollectionChanged += ItemsOnCollectionChanged;
    }

    /// <summary>
    /// The Title of the current note being added
    /// </summary>
    public string? PendingAddNewNoteText { get; set; }

    public ICommand AddNoteCommand { get; }
    public ICommand DeleteNoteCommand { get; }
    public ICommand OpenNoteCommand { get; }
    public ICommand OpenSettingsPageCommand { get; }
    public ICommand OpenCategoryPageCommand { get; }

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

    private void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            Items[i].ListOrder = i;
        }

        _notesRepository.UpdateNoteListOrders(Items.MapList());
    }

    private void AddNote()
    {
        PendingAddNewNoteText = PendingAddNewNoteText?.Trim();
        if (string.IsNullOrWhiteSpace(PendingAddNewNoteText))
        {
            return;
        }

        CreateNote();

        // Reset the input TextBox text
        PendingAddNewNoteText = string.Empty;
    }

    private void CreateNote()
    {
        if (string.IsNullOrWhiteSpace(PendingAddNewNoteText))
        {
            throw new InvalidOperationException("Cannot add note with empty name");
        }

        var notes = _notesRepository.GetActiveNotes();

        var lastListOrder = notes.Any()
            ? notes.Last().ListOrder
            : Constants.DefaultListOrder;

        var note = new Note
        {
            Title = PendingAddNewNoteText,
            Content = string.Empty,
            CreationDate = DateTime.Now,
            ModificationDate = DateTime.Now,
            IsDeleted = false,
            ListOrder = lastListOrder + 1
        };

        note = _notesRepository.AddNote(note);
        Items.Add(note.MapToViewModel());
    }

    private void DeleteNote(NoteViewModel note)
    {
        _notesRepository.DeleteNote(note.Map());
        Items.Remove(note);

        if (ActiveNote?.Id == note.Id)
        {
            ActiveNote = Items.FirstOrDefault();
        }
    }

    /// <summary>
    /// Sets the current category to the specified one.
    /// Ensures that always only one IsSelected property is set to true.
    /// </summary>
    /// <param name="note"></param>
    private void SetActiveNote(NoteViewModel note)
    {
        if (string.IsNullOrEmpty(note?.Title)) return;

        if (ActiveNote != note)
        {
            ActiveNote = note;
            //IoC.CategoryListService.ActiveCategory = null;
        }

        if (AppSettings.Instance.AppWindowSettings.CloseSideMenuOnCategoryChange)
        {
            MediatorOBSOLETE.NotifyClients(ViewModelMessages.SideMenuCloseRequested);
        }

        _mainPageNavigationService.NavigateTo<INoteEditorPage>();

        OnPropertyChanged(nameof(ActiveNote));
    }

    private void OpenSettingsPage()
    {
        _mainPageNavigationService.NavigateTo<ISettingsPage>();
        MediatorOBSOLETE.NotifyClients(ViewModelMessages.SideMenuCloseRequested);
    }

    private void OpenCategoryPage()
    {
        _sideMenuPageNavigationService.NavigateTo<ICategoryListPage>();
    }

    protected override void OnDispose()
    {
        Items.CollectionChanged -= ItemsOnCollectionChanged;
    }

    public void SaveNoteContent()
    {
        if (ActiveNote != null)
        {
            _notesRepository.UpdateNoteContent(ActiveNote.Map());
        }
    }
}