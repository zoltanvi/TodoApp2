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
            private readonly TaskListService _TaskListService;
            private readonly AppViewModel _AppViewModel;
            private readonly IDatabase _Database;
            private readonly OverlayPageService _OverlayPageService;
            private readonly CategoryListService _CategoryListService;
            private readonly NoteListService _NoteListService;
            private readonly MessageService _MessageService;

            private static ApplicationPageManager _Instance;
            public static ApplicationPageManager Instance => _Instance ?? (_Instance = new ApplicationPageManager());

            private readonly Dictionary<ApplicationPage, BaseViewModel> m_ViewModelDictionary;

            public ApplicationPageManager()
            {
                _TaskListService = IoC.TaskListService;
                _AppViewModel = IoC.ApplicationViewModel;
                _Database = IoC.Database;
                _OverlayPageService = IoC.OverlayPageService;
                _CategoryListService = IoC.CategoryListService;
                _NoteListService = IoC.NoteListService;
                _MessageService = IoC.MessageService;

                m_ViewModelDictionary = new Dictionary<ApplicationPage, BaseViewModel>();
            }

            public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                BasePage page = null;
                ApplicationPage applicationPage = (ApplicationPage)value;
                BaseViewModel viewModel = null;

                // Find the appropriate page
                switch (applicationPage)
                {
                    case ApplicationPage.Task:
                    {
                        TaskPageViewModel taskPageViewModel = new TaskPageViewModel(_AppViewModel, _TaskListService, _CategoryListService);

                        viewModel = taskPageViewModel;
                        page = new TaskPage(taskPageViewModel, _TaskListService);

                        break;
                    }
                    case ApplicationPage.Category:
                    {
                        CategoryPageViewModel categoryPageViewModel = new CategoryPageViewModel(
                            _AppViewModel,
                            _Database,
                            _OverlayPageService,
                            _CategoryListService,
                            _MessageService);

                        viewModel = categoryPageViewModel;
                        page = new CategoryPage(categoryPageViewModel);

                        break;
                    }
                    case ApplicationPage.Settings:
                    {
                        SettingsPageViewModel settingsPageViewModel = new SettingsPageViewModel(_AppViewModel);

                        viewModel = settingsPageViewModel;
                        page = new SettingsPage(settingsPageViewModel);

                        break;
                    }
                    case ApplicationPage.NoteList:
                    {
                        NoteListPageViewModel notePageViewModel = new NoteListPageViewModel(
                            _AppViewModel,
                            _Database,
                            _OverlayPageService,
                            _NoteListService,
                            _MessageService);

                        viewModel = notePageViewModel;
                        page = new NoteListPage(notePageViewModel);

                        break;
                    }

                    case ApplicationPage.Note:
                    {
                        NotePageViewModel notePageViewModel = new NotePageViewModel(
                            _AppViewModel,
                            _NoteListService,
                            _Database);

                        viewModel = notePageViewModel;
                        page = new NotePage(notePageViewModel);

                        break;
                    }
                    case ApplicationPage.NotePageSettings:
                    {
                        NotePageSettingsPageViewModel notePageSettingsPageViewModel = new NotePageSettingsPageViewModel();
                        
                        viewModel = notePageSettingsPageViewModel;
                        page = new NotePageSettingsPage(notePageSettingsPageViewModel);

                        break;
                    }
                    case ApplicationPage.TaskItemSettings:
                    {
                        TaskItemSettingsPageViewModel taskItemSettingsPageViewModel = new TaskItemSettingsPageViewModel();

                        viewModel = taskItemSettingsPageViewModel;
                        page = new TaskItemSettingsPage(taskItemSettingsPageViewModel);

                        break;
                    }
                    case ApplicationPage.TaskPageSettings:
                    {
                        TaskPageSettingsPageViewModel taskPageSettingsPageViewModel = new TaskPageSettingsPageViewModel();

                        viewModel = taskPageSettingsPageViewModel;
                        page = new TaskPageSettingsPage(taskPageSettingsPageViewModel);

                        break;
                    }
                    case ApplicationPage.TaskQuickActionsSettings:
                    {
                        TaskQuickActionsSettingsPageViewModel taskQuickActionsSettingsPageViewModel = new TaskQuickActionsSettingsPageViewModel();

                        viewModel = taskQuickActionsSettingsPageViewModel;
                        page = new TaskQuickActionsSettingsPage(taskQuickActionsSettingsPageViewModel);

                        break;
                    }
                    case ApplicationPage.TextEditorQuickActionsSettings:
                    {
                        TextEditorQuickActionsSettingsPageViewModel textEditorQuickActionsSettingsPageViewModel = new TextEditorQuickActionsSettingsPageViewModel();

                        viewModel = textEditorQuickActionsSettingsPageViewModel;
                        page = new TextEditorQuickActionsSettingsPage(textEditorQuickActionsSettingsPageViewModel);

                        break;
                    }
                    case ApplicationPage.ThemeSettings:
                    {
                        ThemeSettingsPageViewModel themeSettingsPageViewModel = new ThemeSettingsPageViewModel(_AppViewModel);

                        viewModel = themeSettingsPageViewModel;
                        page = new ThemeSettingsPage(themeSettingsPageViewModel);

                        break;
                    }
                    case ApplicationPage.WindowSettings:
                    {
                        WindowSettingsPageViewModel windowSettingsPageViewModel = new WindowSettingsPageViewModel();

                        viewModel = windowSettingsPageViewModel;
                        page = new WindowSettingsPage(windowSettingsPageViewModel);

                        break;
                    }
                    case ApplicationPage.ThemeEditorSettings:
                    {
                        ThemeEditorSettingsPageViewModel themeEditorSettingsPageViewModel = new ThemeEditorSettingsPageViewModel(_AppViewModel);

                        viewModel = themeEditorSettingsPageViewModel;
                        page = new ThemeEditorSettingsPage(themeEditorSettingsPageViewModel);

                        break;
                    }
                    default:
                    {
                        applicationPage = ApplicationPage.Invalid;
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