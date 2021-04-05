using System;
using System.Diagnostics;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class OverlayPageService : BaseViewModel
    {
        private ApplicationViewModel Application => IoC.Application;

        /// <summary>
        /// True if the overlay background should be shown
        /// </summary>
        public bool OverlayBackgroundVisible { get; set; }

        /// <summary>
        /// A page content dependent command that executes when the overlay background is clicked.
        /// </summary>
        public ICommand BackgroundClickedCommand { get; private set; }

        public void SetBackgroundClickedAction(Action action)
        {
            BackgroundClickedCommand = new RelayCommand(action);
        }

        public void CloseSideMenu()
        {
            Application.SideMenuVisible = false;
            OverlayBackgroundVisible = false;
        }

        public void ClosePage()
        {
            if (Application.OverlayPageVisible)
            {
                Application.OverlayPageVisible = false;
                OverlayBackgroundVisible = false;

                Application.CloseOverlayPage();

                BackgroundClickedCommand?.Execute(null);
            }

            Application.SideMenuPage = ApplicationPage.Category;
        }

        public void OpenPage(ApplicationPage page, TaskListItemViewModel task = null)
        {
            BaseViewModel viewModel = null;
            bool validPage = page == ApplicationPage.TaskReminder ||
                             page == ApplicationPage.ReminderEditor ||
                             page == ApplicationPage.Notification;

            if (validPage)
            {
                switch (page)
                {
                    case ApplicationPage.TaskReminder:
                        viewModel = new TaskReminderPageViewModel(task);
                        break;
                    case ApplicationPage.ReminderEditor:
                        viewModel = new ReminderEditorPageViewModel(task);
                        break;
                    case ApplicationPage.Notification:
                        viewModel = new NotificationPageViewModel(task);
                        break;
                }

                Application.SideMenuVisible = false;
                OpenOverlayPage(page, viewModel);
            }
            else
            {
                Debugger.Break();
            }
        }

        /// <summary>
        /// Shows the overlay page with the given page.
        /// </summary>
        /// <param name="page">The page to show on the overlay page.</param>
        /// <param name="viewModel">The view model of the page.</param>
        private void OpenOverlayPage(ApplicationPage page, BaseViewModel viewModel)
        {
            Application.GoToOverlayPage(page, true, viewModel);

            OverlayBackgroundVisible = true;
        }
    }
}