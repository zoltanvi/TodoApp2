using System;
using System.Diagnostics;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class OverlayPageService : BaseViewModel, IOverlayPageService
    {
        private readonly AppViewModel m_ApplicationViewModel;
        private readonly IDatabase m_Database;

        public ReminderNotificationService ReminderNotificationService { get; set; }

        /// <summary>
        /// True if the overlay background should be shown
        /// </summary>
        public bool OverlayBackgroundVisible { get; set; }

        /// <summary>
        /// A page content dependent command that executes when the overlay background is clicked.
        /// </summary>
        public ICommand BackgroundClickedCommand { get; private set; }

        public OverlayPageService(AppViewModel applicationViewModel, IDatabase database)
        {
            m_ApplicationViewModel = applicationViewModel;
            m_Database = database;
        }

        public void SetBackgroundClickedAction(Action action)
        {
            BackgroundClickedCommand = new RelayCommand(action);
        }

        public void CloseSideMenu()
        {
            m_ApplicationViewModel.SideMenuVisible = false;
            OverlayBackgroundVisible = false;
        }

        public void ClosePage()
        {
            if (m_ApplicationViewModel.OverlayPageVisible)
            {
                m_ApplicationViewModel.OverlayPageVisible = false;
                OverlayBackgroundVisible = false;

                m_ApplicationViewModel.CloseOverlayPage();

                BackgroundClickedCommand?.Execute(null);
            }

            m_ApplicationViewModel.SideMenuPage = ApplicationPage.Category;
        }

        public void OpenPage(ApplicationPage page, TaskViewModel task = null)
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
                        viewModel = new TaskReminderPageViewModel(task, this, m_Database);
                        break;
                    case ApplicationPage.ReminderEditor:
                        viewModel = new ReminderEditorPageViewModel(task, this, m_Database, ReminderNotificationService);
                        break;
                    case ApplicationPage.Notification:
                        viewModel = new NotificationPageViewModel(task, this);
                        break;
                }

                m_ApplicationViewModel.SideMenuVisible = false;
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
            m_ApplicationViewModel.GoToOverlayPage(page, true, viewModel);

            OverlayBackgroundVisible = true;
        }
    }
}