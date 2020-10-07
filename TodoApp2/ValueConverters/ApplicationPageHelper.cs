using System.Diagnostics;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public static class ApplicationPageHelper
    {
        /// <summary>
        /// Takes a <see cref="ApplicationPage"/> and a view model, if any, and creates the desired page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static BasePage ToBasePage(this ApplicationPage page, object viewModel = null)
        {
            // Find the appropriate page
            switch (page)
            {
                case ApplicationPage.Task:
                    return new TaskPage();

                case ApplicationPage.SideMenu:
                    return new CategoryPage();

                case ApplicationPage.Reminder:
                    return new ReminderPage(viewModel as ReminderPageViewModel);

                case ApplicationPage.Notification:
                    return new NotificationPage(viewModel as NotificationPageViewModel);

                default:
                    Debugger.Break();
                    return null;
            }
        }

        /// <summary>
        /// Converts a <see cref="BasePage"/> to the specific <see cref="ApplicationPage"/> that is for that type of page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static ApplicationPage ToApplicationPage(this BasePage page)
        {
            // Find application page that matches the base page
            if (page is TaskPage)
                return ApplicationPage.Task;
            if (page is CategoryPage)
                return ApplicationPage.SideMenu;
            if (page is ReminderPage)
                return ApplicationPage.Reminder;
            if (page is NotificationPage)
                return ApplicationPage.Notification;

            // Alert developer of issue
            Debugger.Break();
            return default;
        }
    }
}