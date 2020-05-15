using System.Collections.Generic;

namespace TodoApp2.Core
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        private string m_CurrentCategory;

        private bool m_AppSettingsLoadedFirstTime = false;

        /// <summary>
        /// The current page of the application
        /// </summary>
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Task;

        /// <summary>
        /// The side menu content page
        /// </summary>
        public ApplicationPage SideMenuPage { get; } = ApplicationPage.SideMenu;

        /// <summary>
        /// True if the side menu should be shown
        /// </summary>
        public bool SideMenuVisible { get; set; }

        public string CurrentCategory
        {
            get => m_CurrentCategory;
            set
            {
                m_CurrentCategory = value;

                // Notify all listeners about the category change
                Mediator.Instance.NotifyClients(ViewModelMessages.CategoryChanged, value);
            }
        }

        public ApplicationSettings ApplicationSettings { get; } = new ApplicationSettings();

        public void LoadApplicationSettingsOnce()
        {
            if (!m_AppSettingsLoadedFirstTime)
            {
                m_AppSettingsLoadedFirstTime = true;

               LoadApplicationSettings();
            }
        }

        public void LoadApplicationSettings()
        {
            List<SettingsModel> settings = IoC.ClientDatabase.GetSettings();
            ApplicationSettings.LoadEntries(settings);

            CurrentCategory = ApplicationSettings.CurrentCategory;
        }

        public void SaveApplicationSettings()
        {
            ApplicationSettings.CurrentCategory = CurrentCategory;

            List<SettingsModel> settings = ApplicationSettings.GetEntries();
            IoC.ClientDatabase.UpdateSettings(settings);
        }
    }
}
