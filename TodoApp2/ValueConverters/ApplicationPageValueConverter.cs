using System;
using System.Diagnostics;
using System.Globalization;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Find the appropriate page
            switch ((ApplicationPage)value)
            {
                case ApplicationPage.Task:
                    return new TaskPage();

                case ApplicationPage.Category:
                    return new CategoryPage();

                case ApplicationPage.Settings:
                    return new SettingsPage();

                case ApplicationPage.TaskReminder:
                    return new TaskReminderPage();

                case ApplicationPage.ReminderEditor:
                    return new ReminderEditorPage();

                case ApplicationPage.Notification:
                    return new NotificationPage(parameter as NotificationPageViewModel);

                default:
                    //Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}