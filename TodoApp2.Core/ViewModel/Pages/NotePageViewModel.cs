using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace TodoApp2.Core
{
    public class NotePageViewModel : BaseViewModel
    {
        private ApplicationViewModel m_Application;
        private IDatabase m_Database;
        private DispatcherTimer m_Timer;
        private bool m_Saved;
        private bool m_Initialized = false;

        public NotePageViewModel()
        {
        }

        public NotePageViewModel(ApplicationViewModel applicationViewModel, IDatabase database)
        {
            m_Application = applicationViewModel;
            m_Database = database;
            m_Timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            m_Timer.Tick += TimerOnTick;
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

            string propertyName = nameof(ApplicationSettings.NoteContent);

            SettingsModel noteContent = m_Application.ApplicationSettings.GetSetting(propertyName);
            m_Database.UpdateSettings(new List<SettingsModel>() { noteContent });
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
