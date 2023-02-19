using System;
using System.Collections.Generic;
using System.Globalization;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public class ApplicationPageValueConverter : BaseValueConverter
    {
        /// <summary>
        /// Nested singleton manager class.
        /// It is used to dispose the view model of the returned page object if a new page instance is requested.
        /// </summary>
        private class ApplicationPageManager : BaseValueConverter
        {
            private readonly TaskListService m_TaskListService;
            private readonly AppViewModel m_ApplicationViewModel;
            private readonly IDatabase m_Database;
            private readonly OverlayPageService m_OverlayPageService;
            private readonly CategoryListService m_CategoryListService;
            private readonly NoteListService m_NoteListService;
            private readonly MessageService m_MessageService;

            private static ApplicationPageManager s_Instance;
            public static ApplicationPageManager Instance => s_Instance ?? (s_Instance = new ApplicationPageManager());

            private readonly Dictionary<ApplicationPage, BaseViewModel> m_ViewModelDictionary;

            public ApplicationPageManager()
            {
                m_TaskListService = IoC.TaskListService;
                m_ApplicationViewModel = IoC.ApplicationViewModel;
                m_Database = IoC.Database;
                m_OverlayPageService = IoC.OverlayPageService;
                m_CategoryListService = IoC.CategoryListService;
                m_NoteListService = IoC.NoteListService;
                m_MessageService = IoC.MessageService;

                m_ViewModelDictionary = new Dictionary<ApplicationPage, BaseViewModel>();
            }

            public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                BasePage page = null;
                ApplicationPage applicationPage = ApplicationPage.Invalid;
                BaseViewModel viewModel = null;

                // Find the appropriate page
                switch ((ApplicationPage)value)
                {
                    case ApplicationPage.Task:
                    {
                        applicationPage = ApplicationPage.Task;

                        TaskPageViewModel taskPageViewModel = new TaskPageViewModel(m_ApplicationViewModel, m_TaskListService, m_CategoryListService);

                        viewModel = taskPageViewModel;
                        page = new TaskPage(taskPageViewModel, m_TaskListService);

                        break;
                    }
                    case ApplicationPage.Category:
                    {
                        applicationPage = ApplicationPage.Category;

                        CategoryPageViewModel categoryPageViewModel = new CategoryPageViewModel(
                            m_ApplicationViewModel,
                            m_Database,
                            m_OverlayPageService,
                            m_CategoryListService,
                            m_MessageService);

                        viewModel = categoryPageViewModel;
                        page = new CategoryPage(categoryPageViewModel);

                        break;
                    }
                    case ApplicationPage.Settings:
                    {
                        applicationPage = ApplicationPage.Settings;

                        SettingsPageViewModel settingsPageViewModel = new SettingsPageViewModel(m_ApplicationViewModel);

                        viewModel = settingsPageViewModel;
                        page = new SettingsPage(settingsPageViewModel);

                        break;
                    }
                    case ApplicationPage.NoteList:
                    {
                        applicationPage = ApplicationPage.NoteList;

                        NoteListPageViewModel notePageViewModel = new NoteListPageViewModel(
                            m_ApplicationViewModel,
                            m_Database,
                            m_OverlayPageService,
                            m_NoteListService,
                            m_MessageService);

                        viewModel = notePageViewModel;
                        page = new NoteListPage(notePageViewModel);

                        break;
                    }

                    case ApplicationPage.Note:
                    {
                        applicationPage = ApplicationPage.Note;

                        NotePageViewModel notePageViewModel = new NotePageViewModel(
                            m_ApplicationViewModel,
                            m_NoteListService,
                            m_Database);

                        viewModel = notePageViewModel;
                        page = new NotePage(notePageViewModel);

                        break;
                    }
                }

                if (applicationPage != ApplicationPage.Invalid)
                {
                    if (m_ViewModelDictionary.ContainsKey(applicationPage))
                    {
                        BaseViewModel oldViewModel = m_ViewModelDictionary[applicationPage];
                        oldViewModel?.Dispose();
                        m_ViewModelDictionary.Remove(applicationPage);
                    }

                    m_ViewModelDictionary.Add(applicationPage, viewModel);
                }

                return page;
            }
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ApplicationPageManager.Instance.Convert(value, targetType, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ApplicationPageManager.Instance.ConvertBack(value, targetType, parameter, culture);
        }
    }
}