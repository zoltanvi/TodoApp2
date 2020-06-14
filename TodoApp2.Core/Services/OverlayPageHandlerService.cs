using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    public class OverlayPageHandlerService
    {
        private ApplicationViewModel Application => IoC.Application;

        public OverlayPageHandlerService()
        {
            // Listen out for requests to open the overlay background
            Mediator.Instance.Register(OnOverlayBackgroundOpenRequested, ViewModelMessages.OpenOverlayBackgroundRequested);

            // Listen out for requests to open the notification page
            Mediator.Instance.Register(OnNotificationPageOpenRequested, ViewModelMessages.OpenNotificationPageRequested);

            // Listen out for requests to close the notification page
            Mediator.Instance.Register(OnNotificationPageCloseRequested, ViewModelMessages.CloseNotificationPageRequested);

            // Listen out for requests to open the reminder page
            Mediator.Instance.Register(OnReminderPageOpenRequested, ViewModelMessages.OpenReminderPageRequested);

            // Listen out for requests to open the reminder page
            Mediator.Instance.Register(OnReminderPageCloseRequested, ViewModelMessages.CloseReminderPageRequested);
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

        private void OnOverlayBackgroundOpenRequested(object obj)
        {
            Application.OverlayBackgroundVisible = true;
        }

        private void OnNotificationPageOpenRequested(object obj)
        {
            if (obj is TaskListItemViewModel task)
            {
                // Close the side menu regardless whether it was opened or not
                Application.SideMenuVisible = false;

                // Create a view model to pass to the notification page
                BaseViewModel viewModel = new NotificationPageViewModel { NotificationTask = task };

                // Change the overlay page to Notification page
                OpenOverlayPage(ApplicationPage.Notification, viewModel);
            }
        }

        private void OnNotificationPageCloseRequested(object obj)
        {
            CloseEveryOverlay();
        }

        private void OnReminderPageOpenRequested(object obj)
        {
            if (obj is TaskListItemViewModel task)
            {
                // Create a view model to pass to the notification page
                BaseViewModel viewModel = new ReminderPageViewModel(task);

                // Change the overlay page to Reminder page
                OpenOverlayPage(ApplicationPage.Reminder, viewModel);
            }
        }

        private void OnReminderPageCloseRequested(object obj)
        {
            CloseEveryOverlay();
        }
    }
}
