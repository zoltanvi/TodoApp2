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

                case ApplicationPage.SideMenu:
                    return new CategoryPage();

                case ApplicationPage.Options:
                    return new OptionsPage();

                case ApplicationPage.Reminder:
                    return new ReminderPage();

                case ApplicationPage.Notification:
                    return new NotificationPage(parameter as NotificationPageViewModel);

                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
