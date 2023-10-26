using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class NoteListPageViewModel : BaseViewModel
    {
        private readonly IDatabase _database;
        private readonly AppViewModel _appViewModel;
        private readonly OverlayPageService _overlayPageService;
        private readonly NoteListService _noteListService;
        private readonly MessageService _messageService;

        private int _lastRemovedId = int.MinValue;

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

        public NoteListPageViewModel()
        {
        }

        public NoteListPageViewModel(
            AppViewModel applicationViewModel,
            IDatabase database,
            OverlayPageService overlayPageService,
            NoteListService noteListService,
            MessageService messageService)
        {
            _appViewModel = applicationViewModel;
            _database = database;
            _overlayPageService = overlayPageService;
            _noteListService = noteListService;
            _messageService = messageService;

            AddNoteCommand = new RelayCommand(AddNote);
            DeleteNoteCommand = new RelayParameterizedCommand(TrashNote);
            OpenNoteCommand = new RelayParameterizedCommand(OpenNote);
            OpenSettingsPageCommand = new RelayCommand(OpenSettingsPage);
            OpenCategoryPageCommand = new RelayCommand(OpenCategoryPage);

            // Subscribe to the collection changed event for synchronizing with database
            _noteListService.Items.CollectionChanged += ItemsOnCollectionChanged;

            // Load the application settings to update the ActiveNote
            _appViewModel.LoadAppSettingsOnce();
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
                            _database.ReorderNote(newItem, e.NewStartingIndex);
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

            NoteViewModel note = _database.CreateNote(PendingAddNewNoteText);

            Items.Add(note);

            // Reset the input TextBox text
            PendingAddNewNoteText = string.Empty;
        }

        private void TrashNote(object obj)
        {
            if (obj is NoteViewModel note)
            {
                note.Trashed = true;

                Items.Remove(note);

                _database.UpdateNote(note);

                if (ActiveNote.Id == note.Id)
                {
                    ActiveNote = _database.GetValidNotes().FirstOrDefault();
                }
            }
        }

        private void OpenNote(object obj)
        {
            if (obj is NoteViewModel note)
            {
                SetActiveNote(note);
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

                if (IoC.AppSettings.CommonSettings.CloseSideMenuOnCategoryChange)
                {
                    Mediator.NotifyClients(ViewModelMessages.SideMenuCloseRequested);
                }

                _overlayPageService.CloseSideMenu();

                // Change to note page if it wasn't active
                if (_appViewModel.MainPage != ApplicationPage.Note)
                {
                    _appViewModel.MainPage = ApplicationPage.Note;
                }
            }
        }

        /// <summary>
        /// Opens the settings page
        /// </summary>
        private void OpenSettingsPage()
        {
            _appViewModel.OpenSettingsPage();

            Mediator.NotifyClients(ViewModelMessages.SideMenuCloseRequested);
        }

        /// <summary>
        /// Opens the note page
        /// </summary>
        private void OpenCategoryPage()
        {
            _appViewModel.SideMenuPage = ApplicationPage.Category;
        }

        protected override void OnDispose()
        {
            _noteListService.Items.CollectionChanged -= ItemsOnCollectionChanged;
        }
    }
}