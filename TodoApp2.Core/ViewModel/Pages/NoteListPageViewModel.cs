using Modules.Common.DataBinding;
using Modules.Common.DataModels;
using Modules.Common.Navigation;
using Modules.Common.OBSOLETE.Mediator;
using Modules.Common.Services.Navigation;
using Modules.Common.ViewModel;
using Modules.Notes.Repositories;
using Modules.Notes.Repositories.Models;
using Modules.Notes.ViewModels;
using Modules.Settings.Contracts.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using TodoApp2.Common;
using TodoApp2.Core.Mappings;

namespace TodoApp2.Core;

public class NoteListPageViewModel : BaseViewModel
{
    private readonly INotesRepository _notesRepository;
    private readonly AppViewModel _appViewModel;
    private readonly OverlayPageService _overlayPageService;
    private readonly NoteListService _noteListService;
    private readonly MessageService _messageService;
    private readonly IMainPageNavigationService _mainPageNavigationService;
    private readonly ISideMenuPageNavigationService _sideMenuPageNavigationService;

    private int _lastRemovedId = int.MinValue;

    public NoteListPageViewModel(
        AppViewModel applicationViewModel,
        INotesRepository notesRepository,
        OverlayPageService overlayPageService,
        NoteListService noteListService,
        MessageService messageService,
        IMainPageNavigationService mainPageNavigationService,
        ISideMenuPageNavigationService sideMenuPageNavigationService)
    {
        ArgumentNullException.ThrowIfNull(applicationViewModel);
        ArgumentNullException.ThrowIfNull(notesRepository);
        ArgumentNullException.ThrowIfNull(overlayPageService);
        ArgumentNullException.ThrowIfNull(noteListService);
        ArgumentNullException.ThrowIfNull(messageService);
        
        _appViewModel = applicationViewModel;
        _notesRepository = notesRepository;
        _overlayPageService = overlayPageService;
        _noteListService = noteListService;
        _messageService = messageService;
        _mainPageNavigationService = mainPageNavigationService;
        _sideMenuPageNavigationService = sideMenuPageNavigationService;

        AddNoteCommand = new RelayCommand(AddNote);
        DeleteNoteCommand = new RelayParameterizedCommand<NoteViewModel>(TrashNote);
        OpenNoteCommand = new RelayParameterizedCommand<NoteViewModel>(SetActiveNote);
        OpenSettingsPageCommand = new RelayCommand(OpenSettingsPage);
        OpenCategoryPageCommand = new RelayCommand(OpenCategoryPage);

        // Subscribe to the collection changed event for synchronizing with database
        _noteListService.Items.CollectionChanged += ItemsOnCollectionChanged;

        // Load the application settings to update the ActiveNote
        _appViewModel.LoadAppSettingsOnce();
    }

    /// <summary>
    /// The Title of the current note being added
    /// </summary>
    public string PendingAddNewNoteText { get; set; }

    public ICommand AddNoteCommand { get; }
    public ICommand DeleteNoteCommand { get; }
    public ICommand OpenNoteCommand { get; }
    public ICommand OpenSettingsPageCommand { get; }
    public ICommand OpenCategoryPageCommand { get; }

    private ObservableCollection<NoteViewModel> Items => _noteListService.Items;

    private NoteViewModel ActiveNote
    {
        get => _noteListService.ActiveNote;
        set => _noteListService.ActiveNote = value;
    }


    private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
            {
                if (e.NewItems.Count > 0)
                {
                    var newItem = (NoteViewModel)e.NewItems[0];

                    // If the newly added item is the same as the last deleted one,
                    // then this was a drag and drop reorder
                    if (newItem.Id == _lastRemovedId)
                    {
                        for (int i = 0; i < _noteListService.Items.Count; i++)
                        {
                            _noteListService.Items[i].ListOrder = i;
                        }

                        var list = _noteListService.Items.MapList();
                        _notesRepository.UpdateNoteList(list);
                    }

                    _lastRemovedId = int.MinValue;
                }
                break;
            }
            case NotifyCollectionChangedAction.Remove:
            {
                if (e.OldItems.Count > 0)
                {
                    var last = (NoteViewModel)e.OldItems[0];

                    _lastRemovedId = last.Id;
                }
                break;
            }
        }
    }

    private void AddNote()
    {
        // Remove trailing and leading whitespaces
        PendingAddNewNoteText = PendingAddNewNoteText.Trim();

        // If the text is empty or only whitespace, refuse
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
        var notes = _notesRepository.GetActiveNotes();

        var lastListOrder = notes.Any()
            ? notes.Last().ListOrder
            : CommonConstants.DefaultListOrder;

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

    private void TrashNote(NoteViewModel note)
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
        if (!string.IsNullOrEmpty(note?.Title))
        {
            if (ActiveNote != note)
            {
                ActiveNote = note;

                IoC.CategoryListService.ActiveCategory = null;
            }

            if (AppSettings.Instance.AppWindowSettings.CloseSideMenuOnCategoryChange)
            {
                Mediator.NotifyClients(ViewModelMessages.SideMenuCloseRequested);
            }

            _overlayPageService.CloseSideMenu();

            _mainPageNavigationService.NavigateTo<INoteEditorPage>();
        }
    }

    /// <summary>
    /// Opens the settings page
    /// </summary>
    private void OpenSettingsPage()
    {
        Mediator.NotifyClients(ViewModelMessages.OpenSettingsPage);

        Mediator.NotifyClients(ViewModelMessages.SideMenuCloseRequested);
    }

    /// <summary>
    /// Opens the note page
    /// </summary>
    private void OpenCategoryPage()
    {
        _sideMenuPageNavigationService.NavigateTo<ICategoryListPage>();
    }

    protected override void OnDispose()
    {
        _noteListService.Items.CollectionChanged -= ItemsOnCollectionChanged;
    }
}