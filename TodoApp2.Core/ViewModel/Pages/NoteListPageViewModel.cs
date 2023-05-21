using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace TodoApp2.Core
{
    public class NoteListPageViewModel : BaseViewModel
    {
        private readonly IDatabase m_Database;
        private readonly AppViewModel m_AppViewModel;
        private readonly OverlayPageService m_OverlayPageService;
        private readonly NoteListService m_NoteListService;
        private readonly MessageService m_MessageService;

        private int m_LastRemovedId = int.MinValue;

        /// <summary>
        /// The Title of the current note being added
        /// </summary>
        public string PendingAddNewNoteText { get; set; }

        public ICommand AddNoteCommand { get; }
        public ICommand DeleteNoteCommand { get; }
        public ICommand OpenNoteCommand { get; }
        public ICommand OpenSettingsPageCommand { get; }
        public ICommand OpenCategoryPageCommand { get; }

        private ObservableCollection<NoteViewModel> Items => m_NoteListService.Items;

        private NoteViewModel ActiveNote
        {
            get => m_NoteListService.ActiveNote;
            set => m_NoteListService.ActiveNote = value;
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
            m_AppViewModel = applicationViewModel;
            m_Database = database;
            m_OverlayPageService = overlayPageService;
            m_NoteListService = noteListService;
            m_MessageService = messageService;

            AddNoteCommand = new RelayCommand(AddNote);
            DeleteNoteCommand = new RelayParameterizedCommand(TrashNote);
            OpenNoteCommand = new RelayParameterizedCommand(OpenNote);
            OpenSettingsPageCommand = new RelayCommand(OpenSettingsPage);
            OpenCategoryPageCommand = new RelayCommand(OpenCategoryPage);

            // Subscribe to the collection changed event for synchronizing with database
            m_NoteListService.Items.CollectionChanged += ItemsOnCollectionChanged;

            // Load the application settings to update the ActiveNote
            m_AppViewModel.LoadApplicationSettingsOnce();

            // Subscribe to the theme changed event to repaint the list items when it happens
            Mediator.Register(OnThemeChanged, ViewModelMessages.ThemeChanged);
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
                        if (newItem.Id == m_LastRemovedId)
                        {
                            m_Database.ReorderNote(newItem, e.NewStartingIndex);
                        }

                        m_LastRemovedId = int.MinValue;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems.Count > 0)
                    {
                        var last = (NoteViewModel)e.OldItems[0];

                        m_LastRemovedId = last.Id;
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

            NoteViewModel note = m_Database.CreateNote(PendingAddNewNoteText);

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

                m_Database.UpdateNote(note);

                if (ActiveNote.Id == note.Id)
                {
                    ActiveNote = m_Database.GetValidNotes().FirstOrDefault();
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

                if (m_AppViewModel.ApplicationSettings.CloseSideMenuOnCategoryChange)
                {
                    Mediator.NotifyClients(ViewModelMessages.SideMenuCloseRequested);
                }

                m_OverlayPageService.CloseSideMenu();

                // Change to note page if it wasn't active
                if (m_AppViewModel.MainPage != ApplicationPage.Note)
                {
                    m_AppViewModel.MainPage = ApplicationPage.Note;
                }
            }
        }

        /// <summary>
        /// Opens the settings page
        /// </summary>
        private void OpenSettingsPage()
        {
            m_AppViewModel.OpenSettingsPage();

            Mediator.NotifyClients(ViewModelMessages.SideMenuCloseRequested);
        }

        /// <summary>
        /// Opens the note page
        /// </summary>
        private void OpenCategoryPage()
        {
            m_AppViewModel.SideMenuPage = ApplicationPage.Category;
        }

        /// <summary>
        /// Forces the UI to repaint the list items when the theme changes
        /// </summary>
        /// <param name="obj"></param>
        private void OnThemeChanged(object obj)
        {
            //Save the current items
            List<NoteViewModel> itemsBackup = new List<NoteViewModel>(Items);

            //Clear the items and add back the cleared items to refresh the list(repaint)
            Items.Clear();
            Items.AddRange(itemsBackup);
        }

        protected override void OnDispose()
        {
            m_NoteListService.Items.CollectionChanged -= ItemsOnCollectionChanged;

            Mediator.Deregister(OnThemeChanged, ViewModelMessages.ThemeChanged);
        }
    }
}