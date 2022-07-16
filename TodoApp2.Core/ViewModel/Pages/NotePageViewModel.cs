using System.Collections.Generic;

namespace TodoApp2.Core
{
    public class NotePageViewModel : BaseViewModel
    {
        ApplicationViewModel m_Application;
        IDatabase m_Database;

        public NotePageViewModel()
        {
        }

        public NotePageViewModel(ApplicationViewModel applicationViewModel, IDatabase database)
        {
            m_Application = applicationViewModel;
            m_Database = database;
        }

        public void SaveNoteContent()
        {
            string propertyName = nameof(ApplicationSettings.NoteContent);

            SettingsModel noteContent = m_Application.ApplicationSettings.GetSetting(propertyName);
            m_Database.UpdateSettings(new List<SettingsModel>() { noteContent });
            m_Application.SaveIconVisible = !m_Application.SaveIconVisible;
        }
    }
}
