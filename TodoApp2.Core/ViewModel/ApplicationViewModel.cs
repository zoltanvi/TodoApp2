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
    public class ApplicationViewModel : BaseViewModel
    {
        private bool m_AppSettingsLoadedFirstTime;

        private readonly ClientDatabase m_ClientDatabase;
        private readonly OverlayPageService m_OverlayPageService;

        /// <summary>
        /// The sliding side menu content page
        /// </summary>
        public ApplicationPage SideMenuPage { get; set; }

        /// <summary>
        /// True if the side menu should be shown
        /// </summary>
        public bool SideMenuVisible { get; set; }

        /// <summary>
        /// The current page of the application
        /// </summary>
        public ApplicationPage CurrentPage { get; set; }

        /// <summary>
        /// The view model to use for the current page when the Page changes
        /// NOTE: This is not a live up-to-date view model of the current page
        ///       it is simply used to set the view model of the current page
        ///       at the time it changes
        /// </summary>
        public IBaseViewModel CurrentPageViewModel { get; set; }

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
        public ApplicationSettings ApplicationSettings { get; } = new ApplicationSettings();

        /// <summary>
        /// Command for toggle between opened and closed state for the side menu
        /// </summary>
        public ICommand ToggleSideMenuCommand { get; }

        public ApplicationViewModel(ClientDatabase clientDatabase, OverlayPageService overlayPageService)
        {
            m_ClientDatabase = clientDatabase;
            m_OverlayPageService = overlayPageService;

            ToggleSideMenuCommand = new RelayCommand(ToggleSideMenu);
            
            // Load the application settings to update the CurrentCategory before querying the tasks
            LoadApplicationSettingsOnce();
            
            ApplicationSettings.PropertyChanged += OnApplicationSettingsPropertyChanged;
        }

        /// <summary>
        /// Navigates the main page to the specified page.
        /// </summary>
        /// <param name="page">The page to go to</param>
        /// <param name="viewModel">The view model to set</param>
        public void GoToPage(ApplicationPage page, IBaseViewModel viewModel = null)
        {
            CurrentPageViewModel = viewModel;

            // See if page has changed
            bool different = CurrentPage != page;

            CurrentPage = page;

            // If the page hasn't changed, fire off notification
            // So pages still update if just the view model has changed
            if (!different)
            {
                //OnPropertyChanged(nameof(CurrentPage));
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

        private void ToggleSideMenu()
        {
            OpenCloseSideMenu(!SideMenuVisible);
        }

        /// <summary>
        /// Opens or closes the side menu.
        /// </summary>
        /// <param name="shouldOpen">True if the side menu should be opened, false if should be closed.</param>
        private void OpenCloseSideMenu(bool shouldOpen)
        {
            if (shouldOpen)
            {
                m_OverlayPageService.SetBackgroundClickedAction(ToggleSideMenu);
            }

            m_OverlayPageService.ClosePage();
            SideMenuVisible = shouldOpen;
            m_OverlayPageService.OverlayBackgroundVisible = shouldOpen;
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
            List<SettingsModel> settings = m_ClientDatabase.GetSettings();
            ApplicationSettings.SetSettings(settings);

            // If the application is closed while no category was selected,
            // set the first valid category as selected.
            // This can happen if the settings page were open during closing
            if (string.IsNullOrEmpty(ApplicationSettings.CurrentCategory))
            {
                ApplicationSettings.CurrentCategory = m_ClientDatabase.GetActiveCategories().FirstOrDefault()?.Name;
            }
        }

        public void SaveApplicationSettings()
        {
            List<SettingsModel> settings = ApplicationSettings.GetSettings();
            m_ClientDatabase.UpdateSettings(settings);
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
        }
    }
}