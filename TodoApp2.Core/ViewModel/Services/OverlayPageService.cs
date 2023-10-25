using System;
using System.Diagnostics;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class OverlayPageService : BaseViewModel, IOverlayPageService
    {
        private readonly AppViewModel _appViewModel;
        private readonly IDatabase _database;

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
            _appViewModel = applicationViewModel;
            _database = database;
        }

        public void SetBackgroundClickedAction(Action action)
        {
            BackgroundClickedCommand = new RelayCommand(action);
        }

        // TODO: Remove side menu handling from here
        public void CloseSideMenu()
        {
            _appViewModel.SideMenuVisible = false;
            OverlayBackgroundVisible = false;
        }

        public void ClosePage()
        {
            if (_appViewModel.OverlayPageVisible)
            {
                _appViewModel.OverlayPageVisible = false;
                OverlayBackgroundVisible = false;

                _appViewModel.CloseOverlayPage();

                BackgroundClickedCommand?.Execute(null);
            }
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
                        viewModel = new TaskReminderPageViewModel(task, this, _database);
                        break;
                    case ApplicationPage.ReminderEditor:
                        viewModel = new ReminderEditorPageViewModel(task, this, _database, ReminderNotificationService);
                        break;
                    case ApplicationPage.Notification:
                        viewModel = new NotificationPageViewModel(task, this);
                        break;
                }

                _appViewModel.SideMenuVisible = false;
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
            _appViewModel.GoToOverlayPage(page, true, viewModel);

            OverlayBackgroundVisible = true;
        }
    }
}