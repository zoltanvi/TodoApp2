using System.Collections.Generic;

namespace TodoApp2.Core
{
    public class NotePageViewModel : BaseViewModel
    {

        public void SaveNoteContent()
        {
            ApplicationViewModel application = IoC.ApplicationViewModel;
            string propertyName = nameof(ApplicationSettings.NoteContent);

            SettingsModel noteContent = application.ApplicationSettings.GetSetting(propertyName);
            IoC.Database.UpdateSettings(new List<SettingsModel>() { noteContent });
            application.SaveIconVisible = !application.SaveIconVisible;
        }
    }
}
