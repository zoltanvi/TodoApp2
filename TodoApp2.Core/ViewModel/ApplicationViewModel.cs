using System.Collections.Generic;
using System.Windows.Input;

namespace TodoApp2.Core
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        private bool m_AppSettingsLoadedFirstTime = false;

        private ClientDatabase ClientDatabase => IoC.ClientDatabase;
        private OverlayPageService OverlayPageService => IoC.OverlayPageService;
        private CategoryListService CategoryListService => IoC.CategoryListService;

        /// <summary>
        /// The sliding side menu content page
        /// </summary>
        public ApplicationPage SideMenuPage { get; } = ApplicationPage.SideMenu;

        /// <summary>
        /// True if the side menu should be shown
        /// </summary>
        public bool SideMenuVisible { get; set; }

        /// <summary>
        /// The current page of the application
        /// </summary>
        public ApplicationPage CurrentPage { get; private set; }

        /// <summary>
        /// The view model to use for the current page when the CurrentPage changes
        /// NOTE: This is not a live up-to-date view model of the current page
        ///       it is simply used to set the view model of the current page
        ///       at the time it changes
        /// </summary>
        public BaseViewModel CurrentPageViewModel { get; set; }

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
        /// Always on top
        /// </summary>
        public bool IsAlwaysOnTop { get; set; }

        /// <summary>
        /// Command for toggle between opened and closed state for the side menu
        /// </summary>
        public ICommand ToggleSideMenuCommand { get; }

        public ApplicationViewModel()
        {
            ToggleSideMenuCommand = new RelayCommand(ToggleSideMenu);
            CurrentPage = ApplicationPage.Task;
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
                OverlayPageService.SetBackgroundClickedAction(ToggleSideMenu);
            }

            OverlayPageService.ClosePage();
            SideMenuVisible = shouldOpen;
            OverlayPageService.OverlayBackgroundVisible = shouldOpen;
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
            List<SettingsModel> settings = ClientDatabase.GetSettings();
            ApplicationSettings.SetSettings(settings);

            CategoryListService.CurrentCategory = ApplicationSettings.CurrentCategory;
        }

        public void SaveApplicationSettings()
        {
            ApplicationSettings.CurrentCategory = CategoryListService.CurrentCategory;

            List<SettingsModel> settings = ApplicationSettings.GetSettings();
            ClientDatabase.UpdateSettings(settings);
        }
    }
}