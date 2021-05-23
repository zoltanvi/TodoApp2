using System;
using System.Globalization;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
    {
        private readonly TaskListService m_TaskListService;
        private readonly ApplicationViewModel m_ApplicationViewModel;
        private readonly IDatabase m_Database;
        private readonly OverlayPageService m_OverlayPageService;
        private readonly CategoryListService m_CategoryListService;

        public ApplicationPageValueConverter()
        {
            m_TaskListService = IoC.TaskListService;
            m_ApplicationViewModel = IoC.ApplicationViewModel;
            m_Database = IoC.Database;
            m_OverlayPageService = IoC.OverlayPageService;
            m_CategoryListService = IoC.CategoryListService;
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Find the appropriate page
            switch ((ApplicationPage)value)
            {
                case ApplicationPage.Task:
                    return new TaskPage(new TaskPageViewModel(m_TaskListService, m_CategoryListService, m_Database), m_TaskListService);

                case ApplicationPage.Category:
                    return new CategoryPage(new CategoryPageViewModel(m_ApplicationViewModel, m_Database,
                        m_OverlayPageService, m_CategoryListService));

                case ApplicationPage.Settings:
                    return new SettingsPage(new SettingsPageViewModel(m_ApplicationViewModel));

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