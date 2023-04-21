using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TodoApp2.Core
{
    public class NoteListService : BaseViewModel
    {
        private AppViewModel m_AppViewModel;
        private IDatabase m_Database;
        private NoteViewModel m_ActiveNote;

        public ObservableCollection<NoteViewModel> Items { get; set; }
        
        public NoteViewModel ActiveNote
        {
            get => m_ActiveNote;
            set
            {
                SaveNoteContent();
                m_ActiveNote = value;
                m_AppViewModel.ApplicationSettings.ActiveNoteId = value?.Id ?? -1;
                Mediator.NotifyClients(ViewModelMessages.NoteChanged);
            }
        }

        public NoteListService(AppViewModel appViewModel, IDatabase database)
        {
            m_AppViewModel = appViewModel;
            m_Database = database;

            List<NoteViewModel> notes = m_Database.GetValidNotes();
            Items = new ObservableCollection<NoteViewModel>(notes);

            m_ActiveNote = m_Database.GetNote(m_AppViewModel.ApplicationSettings.ActiveNoteId);
        }

        public void SaveNoteContent()
        {
            m_Database.UpdateNote(ActiveNote);
        }
    }
}
