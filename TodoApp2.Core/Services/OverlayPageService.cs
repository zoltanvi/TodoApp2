namespace TodoApp2.Core
{
    public class OverlayPageService
    {
        private static OverlayPageService m_Instance;

        public static OverlayPageService Instance = m_Instance ?? (m_Instance = new OverlayPageService());

        private ApplicationViewModel Application => IoC.Application;

        private OverlayPageService()
        {
        }

        /// <summary>
        /// Closes the side menu, the overlay page and hides the overlay background
        /// regardless of their previous state
        /// </summary>
        public void CloseEveryOverlay()
        {
            Application.OverlayBackgroundVisible = false;

            // Notify all listeners about the background close
            Mediator.Instance.NotifyClients(ViewModelMessages.OverlayBackgroundClosed);

            // When the background is closed the side menu and the overlay page should be closed as well
            // regardless of it was opened or closed before
            Application.SideMenuVisible = false;
            Application.OverlayPageVisible = false;
        }

        /// <summary>
        /// Shows the overlay background
        /// </summary>
        public void ShowOverlayBackground()
        {
            Application.OverlayBackgroundVisible = true;
        }

        /// <summary>
        /// Opens the notification page
        /// </summary>
        /// <param name="task">The task the notification came from.</param>
        public void OpenNotificationPage(TaskListItemViewModel task)
        {
            if (task != null)
            {
                // Close the side menu regardless whether it was opened or not
                Application.SideMenuVisible = false;

                // Create a view model to pass to the notification page
                BaseViewModel viewModel = new NotificationPageViewModel(task);

                // Change the overlay page to Notification page
                OpenOverlayPage(ApplicationPage.Notification, viewModel);
            }
        }

        /// <summary>
        /// Closes the notification page
        /// </summary>
        public void CloseNotificationPage()
        {
            CloseEveryOverlay();
        }

        /// <summary>
        /// Opens the reminder page
        /// </summary>
        /// <param name="task">The task to set a reminder for.</param>
        public void OpenReminderPage(TaskListItemViewModel task)
        {
            if (task != null)
            {
                // Create a view model to pass to the notification page
                BaseViewModel viewModel = new ReminderPageViewModel(task);

                // Change the overlay page to Reminder page
                OpenOverlayPage(ApplicationPage.Reminder, viewModel);
            }
        }

        /// <summary>
        /// Closes the reminder page
        /// </summary>
        public void CloseReminderPage()
        {
            CloseEveryOverlay();
        }

        /// <summary>
        /// Shows the overlay page with the given page.
        /// </summary>
        /// <param name="page">The page to show on the overlay page.</param>
        /// <param name="viewModel">The view model of the page.</param>
        private void OpenOverlayPage(ApplicationPage page, BaseViewModel viewModel)
        {
            Application.GoToOverlayPage(page, true, viewModel);

            Application.OverlayBackgroundVisible = true;
        }
    }
}