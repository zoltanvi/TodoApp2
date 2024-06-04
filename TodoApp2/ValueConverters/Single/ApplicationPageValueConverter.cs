using Microsoft.Extensions.DependencyInjection;
using Modules.Common.DataModels;
using Modules.Common.ViewModel;
using Modules.Common.Views.Pages;
using Modules.Common.Views.ValueConverters;
using Modules.Notes.Repositories;
using Modules.Settings.Views.Pages;
using System;
using System.Collections.Generic;
using System.Globalization;
using TodoApp2.Core;
using TodoApp2.Persistence;

namespace TodoApp2;

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
        private readonly TaskListService _taskListService;
        private readonly AppViewModel _appViewModel;
        private readonly IAppContext _context;
        private readonly OverlayPageService _overlayPageService;
        private readonly CategoryListService _categoryListService;
        private readonly NoteListService _noteListService;
        private readonly MessageService _messageService;

        private static ApplicationPageManager _Instance;
        public static ApplicationPageManager Instance => _Instance ?? (_Instance = new ApplicationPageManager());

        private readonly Dictionary<ApplicationPage, BaseViewModel> _viewModelDictionary;

        public ApplicationPageManager()
        {
            _taskListService = IoC.TaskListService;
            _appViewModel = IoC.AppViewModel;
            _context = IoC.Context;
            _overlayPageService = IoC.OverlayPageService;
            _categoryListService = IoC.CategoryListService;
            _noteListService = IoC.NoteListService;
            _messageService = IoC.MessageService;

            _viewModelDictionary = new Dictionary<ApplicationPage, BaseViewModel>();
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
                    var taskPageViewModel = new TaskPageViewModel(
                        _appViewModel,
                        _taskListService,
                        _categoryListService);

                    viewModel = taskPageViewModel;
                    page = new TaskPage(taskPageViewModel, _taskListService);

                    break;
                }
                case ApplicationPage.Category:
                {
                    var categoryPageViewModel = new CategoryPageViewModel(
                        _appViewModel,
                        _context,
                        _overlayPageService,
                        _categoryListService,
                        _taskListService,
                        _messageService);

                    viewModel = categoryPageViewModel;
                    page = new CategoryPage(categoryPageViewModel);

                    break;
                }
                case ApplicationPage.NoteList:
                {
                    var notePageViewModel = new NoteListPageViewModel(
                        _appViewModel,
                        IoC.ServiceProvider.GetService<INotesRepository>(),
                        _overlayPageService,
                        _noteListService,
                        _messageService);

                    viewModel = notePageViewModel;
                    page = new NoteListPage(notePageViewModel);

                    break;
                }
                case ApplicationPage.Note:
                {
                    var notePageViewModel = new NotePageViewModel(
                        _appViewModel,
                        _noteListService);

                    viewModel = notePageViewModel;
                    page = new NotePage(notePageViewModel);

                    break;
                }
                case ApplicationPage.RecycleBin:
                {
                    var recycleBinPageViewModel = new RecycleBinPageViewModel(
                        _appViewModel, 
                        _taskListService,
                        _categoryListService);
                    
                    viewModel = recycleBinPageViewModel;
                    page = new RecycleBinPage(recycleBinPageViewModel);

                    break;
                }
            }

            if (page != null)
            {
                if (_viewModelDictionary.ContainsKey(applicationPage))
                {
                    BaseViewModel oldViewModel = _viewModelDictionary[applicationPage];
                    oldViewModel?.Dispose();
                    _viewModelDictionary.Remove(applicationPage);
                }

                _viewModelDictionary.Add(applicationPage, viewModel);
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