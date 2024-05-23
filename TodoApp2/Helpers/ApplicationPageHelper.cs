using Modules.Common.DataModels;
using System.Diagnostics;
using TodoApp2.Core;

namespace TodoApp2;

/// <summary>
/// Converts the <see cref="ApplicationPage"/> to an actual view/page
/// These methods are currently used for the overlay pages only!
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
            case ApplicationPage.TaskReminder:
                return new TaskReminderPage(viewModel as TaskReminderPageViewModel);

            case ApplicationPage.ReminderEditor:
                return new ReminderEditorPage(viewModel as ReminderEditorPageViewModel);

            case ApplicationPage.Notification:
                return new NotificationPage(viewModel as NotificationPageViewModel);

            default:
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
        if (page is TaskReminderPage)
            return ApplicationPage.TaskReminder;

        if (page is ReminderEditorPage)
            return ApplicationPage.ReminderEditor;

        if (page is NotificationPage)
            return ApplicationPage.Notification;

        Debugger.Break();
        return default;
    }
}