namespace TodoApp2.Core
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {

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

        public string CurrentCategory { get; set; }

        public bool OptimizePerformance { get; set; } = false;

    }
}
