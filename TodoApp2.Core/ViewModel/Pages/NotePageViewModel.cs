using System;
using System.Windows.Threading;

namespace TodoApp2.Core
{
    public class NotePageViewModel : BaseViewModel
    {
        private AppViewModel m_Application;
        private NoteListService m_NoteListService;
        private IDatabase m_Database;
        private DispatcherTimer m_Timer;
        private bool m_Saved;
        private bool m_Initialized = false;

        public NotePageViewModel()
        {
        }

        public bool IsNoteExists { get; private set; }

        public NotePageViewModel(
            AppViewModel appViewModel, 
            NoteListService noteListService, 
            IDatabase database)
        {
            m_Application = appViewModel;
            m_NoteListService = noteListService;
            m_Database = database;
            m_Timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 3) };
            m_Timer.Tick += TimerOnTick;

            IsNoteExists = m_NoteListService.ActiveNote != null;

            Mediator.Register(OnNoteChanged, ViewModelMessages.NoteChanged);
        }

        private void OnNoteChanged(object obj)
        {
            IsNoteExists = m_NoteListService.ActiveNote != null;
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            if (!m_Saved)
            {
                SaveNoteContent();
            }
        }

        public void SaveNoteContent()
        {
            m_Saved = true;
            m_Timer.Stop();

            m_NoteListService.SaveNoteContent();
            
            m_Application.SaveIconVisible = !m_Application.SaveIconVisible;
        }

        public void NoteContentChanged()
        {
            if (m_Initialized)
            {
                m_Saved = false;
                m_Timer.Stop();
                m_Timer.Start();
            }
            else
            {
                m_Initialized = true;
            }
        }
    }
}
