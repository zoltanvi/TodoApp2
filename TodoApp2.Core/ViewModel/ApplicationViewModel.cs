using System.Collections.Generic;

namespace TodoApp2.Core
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        private static List<bool> getCalls = new List<bool>();
        private static List<bool> setCalls = new List<bool>();
        private bool m_SideMenuVisible = false;

        /// <summary>
        /// The current page of the application
        /// </summary>
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Task;

        /// <summary>
        /// True if the side menu should be shown
        /// </summary>
        public bool SideMenuVisible
        {
            get
            {
                getCalls.Add(m_SideMenuVisible);
                return m_SideMenuVisible;
            }
            set
            {
                setCalls.Add(value);
                m_SideMenuVisible = value;
            }
        }
    }
}
