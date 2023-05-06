using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace TodoApp2.Core
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class AppViewModel : BaseViewModel
    {
        private bool m_AppSettingsLoadedFirstTime;
        private IUIScaler m_UiScaler;
        private readonly IDatabase m_Database;

        public IOverlayPageService OverlayPageService { get; set; }

        /// <summary>
        /// The sliding side menu content page
        /// </summary>
        public ApplicationPage SideMenuPage
        {
            get => ApplicationSettings.SideMenuPage;
            set => ApplicationSettings.SideMenuPage = value;
        }

        /// <summary>
        /// True if the side menu should be shown
        /// </summary>
        public bool SideMenuVisible { get; set; }

        /// <summary>
        /// The current page of the application
        /// </summary>
        public ApplicationPage MainPage { get; set; }

        /// <summary>
        /// The view model to use for the current page when the Page changes
        /// NOTE: This is not a live up-to-date view model of the current page
        ///       it is simply used to set the view model of the current page
        ///       at the time it changes
        /// </summary>
        public IBaseViewModel MainPageViewModel { get; set; }

        /// <summary>
        /// The overlay panel content page
        /// </summary>
        public ApplicationPage OverlayPage { get; private set; }

        /// <summary>
        /// The view model to use for the current overlay page when the OverlayPage changes
        /// NOTE: This is not a live up-to-date view model of the current page
        ///       it is simply used to set the view model of the current page
        ///       at the time it changes
        /// </summary>
        public IBaseViewModel OverlayPageViewModel { get; set; }

        /// <summary>
        /// True if the overlay page should be shown
        /// </summary>
        public bool OverlayPageVisible { get; set; }

        /// <summary>
        /// The settings for the whole application
        /// </summary>
        public ApplicationSettings ApplicationSettings { get; }

        /// <summary>
        /// The value change triggers the animation
        /// </summary>
        public bool SaveIconVisible { get; set; }

        public AppViewModel(IDatabase database, IUIScaler uiScaler)
        {
            m_Database = database;
            m_UiScaler = uiScaler;

            ApplicationSettings = new ApplicationSettings(m_UiScaler);

            // Load the application settings to update the ActiveCategoryId before querying the tasks
            LoadApplicationSettingsOnce();

            ApplicationSettings.PropertyChanged += OnApplicationSettingsPropertyChanged;
        }

        /// <summary>
        /// Updates the Main Page based on what is stored in the application settings for active category or active note.
        /// </summary>
        public void UpdateMainPage()
        {
            // The ActiveCategoryId and the ActiveNoteId must be mutually exclusive,
            // meaning that one or the other is set to -1 at all times, but never both at once.
            MainPage = ApplicationSettings.ActiveCategoryId != -1
                ? ApplicationPage.Task
                : ApplicationPage.Note;
        }

        /// <summary>
        /// Changes the Main Page to the settings page or changes it back if the settings page was opened.
        /// </summary>
        public void ToggleSettingsPage()
        {
            if (MainPage == ApplicationPage.Settings)
            {
                UpdateMainPage();
            }
            else
            {
                MainPage = ApplicationPage.Settings;
            }
        }

        /// <summary>
        /// Updates the SideMenu Page based on what is stored in the application settings for active category or active note.
        /// </summary>
        public void UpdateSideMenuPage()
        {
            // The ActiveCategoryId and the ActiveNoteId must be mutually exclusive,
            // meaning that one or the other is set to -1 at all times, but never both at once.
            SideMenuPage = ApplicationSettings.ActiveCategoryId != -1
                ? ApplicationPage.Category
                : ApplicationPage.NoteList;
        }

        /// <summary>
        /// Navigates the main page to the specified page.
        /// </summary>
        /// <param name="page">The page to go to</param>
        /// <param name="viewModel">The view model to set</param>
        public void GoToPage(ApplicationPage page, IBaseViewModel viewModel = null)
        {
            MainPageViewModel = viewModel;

            // See if page has changed
            bool different = MainPage != page;

            MainPage = page;

            // If the page hasn't changed, fire off notification
            // So pages still update if just the view model has changed
            if (!different)
            {
                //OnPropertyChanged(nameof(MainPage));
            }
        }

        /// <summary>
        /// Navigates the overlay page to the specified page
        /// </summary>
        /// <param name="page">The page to go to</param>
        /// <param name="visible">Shows the page if true</param>
        /// <param name="viewModel">The view model, if any, to set explicitly to the new page</param>
        public void GoToOverlayPage(ApplicationPage page, bool visible = true, IBaseViewModel viewModel = null)
        {
            // Always hide side menu if we are changing pages
            SideMenuVisible = false;

            // Set the view model
            OverlayPageViewModel = viewModel;

            // See if page has changed
            bool different = OverlayPage != page;

            // Set the current overlay page
            OverlayPage = page;

            // Show or hide the page
            OverlayPageVisible = visible;

            // If the page hasn't changed, fire off notification
            // So pages still update if just the view model has changed
            if (!different)
            {
                OnPropertyChanged(nameof(OverlayPage));
            }
        }

        /// <summary>
        /// Closes the overlay page and disposes its viewModel
        /// </summary>
        public void CloseOverlayPage()
        {
            OverlayPage = ApplicationPage.Invalid;
            if (OverlayPageViewModel is IDisposable viewModel)
            {
                viewModel.Dispose();
            }
        }

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
            List<Setting> settings = m_Database.GetSettings();
            ApplicationSettings.SetSettings(settings);

            // Must be set to always check the registry on startup
            IoC.AutoRunService.RunAtStartup = ApplicationSettings.RunAtStartup;
        }

        public void SaveApplicationSettings()
        {
            List<Setting> settings = ApplicationSettings.GetSettings();
            m_Database.UpdateSettings(settings);
        }

        /// <summary>
        /// Notifies clients that an application theme change was requested
        /// </summary>
        private void OnApplicationSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ApplicationSettings.ActiveTheme))
            {
                Mediator.NotifyClients(ViewModelMessages.ThemeChangeRequested);
            }
            else if (e.PropertyName == nameof(ApplicationSettings.RunAtStartup))
            {
                IoC.AutoRunService.RunAtStartup = ApplicationSettings.RunAtStartup;
            }
        }
    }
}