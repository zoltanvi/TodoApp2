using System.Collections.Generic;

namespace TodoApp2.Core
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        private string m_CurrentCategory;

        private bool m_AppSettingsLoadedFirstTime = false;

        /// <summary>
        /// The current page of the application
        /// </summary>
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Task;

        /// <summary>
        /// The side menu content page
        /// </summary>
        public ApplicationPage SideMenuPage { get; } = ApplicationPage.SideMenu;

        /// <summary>
        /// The overlay panel content page
        /// </summary>
        public ApplicationPage OverlayPage { get; set; } = ApplicationPage.Reminder;

        /// <summary>
        /// True if the side menu should be shown
        /// </summary>
        public bool SideMenuVisible { get; set; }

        /// <summary>
        /// True if the overlay page should be shown
        /// </summary>
        public bool OverlayPageVisible { get; set; }

        /// <summary>
        /// The task that needs to be shown in the notification
        /// </summary>
        public TaskListItemViewModel NotificationTask { get; set; }

        public string NotificationTaskCategory
        {
            get
            {
                if (NotificationTask != null)
                {
                    var cat = IoC.ClientDatabase.GetCategory(NotificationTask.CategoryId);
                    return cat.Name;
                }

                return "-- Category not found --";
            }
        }

        public string CurrentCategory
        {
            get => m_CurrentCategory;
            set
            {
                m_CurrentCategory = value;

                // Notify all listeners about the category change
                Mediator.Instance.NotifyClients(ViewModelMessages.CategoryChanged, value);
            }
        }

        public ApplicationSettings ApplicationSettings { get; } = new ApplicationSettings();

        public void TurnOffReminderOnNotificationTask()
        {
            if (NotificationTask != null)
            {
                NotificationTask.IsReminderOn = false;
                IoC.ClientDatabase.UpdateTask(NotificationTask);
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
            List<SettingsModel> settings = IoC.ClientDatabase.GetSettings();
            ApplicationSettings.SetSettings(settings);

            CurrentCategory = ApplicationSettings.CurrentCategory;
        }

        public void SaveApplicationSettings()
        {
            ApplicationSettings.CurrentCategory = CurrentCategory;

            List<SettingsModel> settings = ApplicationSettings.GetSettings();
            IoC.ClientDatabase.UpdateSettings(settings);
        }
    }
}
