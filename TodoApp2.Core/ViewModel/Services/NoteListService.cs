using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TodoApp2.Core
{
    public class NoteListService : BaseViewModel
    {
        private AppViewModel _appViewModel;
        private IDatabase _database;
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

        public NoteListService(AppViewModel appViewModel, IDatabase database)
        {
            _appViewModel = appViewModel;
            _database = database;

            List<NoteViewModel> notes = _database.GetValidNotes();
            Items = new ObservableCollection<NoteViewModel>(notes);

            _activeNote = _database.GetNote(IoC.AppSettings.SessionSettings.ActiveNoteId);
        }

        public void SaveNoteContent()
        {
            _database.UpdateNote(ActiveNote);
        }
    }
}
