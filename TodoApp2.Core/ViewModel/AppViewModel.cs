﻿using Modules.Common.DataModels;
using Modules.Common.Navigation;
using Modules.Common.OBSOLETE.Mediator;
using Modules.Common.Services.Navigation;
using Modules.Common.ViewModel;
using Modules.Settings.Contracts.ViewModels;
using Modules.Settings.Services;
using System;
using System.ComponentModel;
using TodoApp2.Common;
using TodoApp2.Persistence;

namespace TodoApp2.Core;

/// <summary>
/// The application state as a view model
/// </summary>
public class AppViewModel : BaseViewModel
{
    private bool _appSettingsLoadedFirstTime;
    private IUIScaler _uiScaler;
    private IAppSettingsService _settingsPopulator;
    private IServiceProvider _serviceProvider;
    private readonly IAppContext _context;
    private readonly IMainPageNavigationService _mainPageNavigationService;
    private readonly ISideMenuPageNavigationService _sideMenuPageNavigationService;

    public AppViewModel(
        IAppContext context,
        UIScaler uiScaler,
        IAppSettingsService settingsPopulator,
        IServiceProvider serviceProvider,
        IMainPageNavigationService mainPageNavigationService,
        ISideMenuPageNavigationService sideMenuPageNavigationService)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(uiScaler);
        ArgumentNullException.ThrowIfNull(settingsPopulator);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _context = context;
        _uiScaler = uiScaler;
        _settingsPopulator = settingsPopulator;
        _serviceProvider = serviceProvider;
        _mainPageNavigationService = mainPageNavigationService;
        _sideMenuPageNavigationService = sideMenuPageNavigationService;

        // Load the application settings to update the ActiveCategoryId before querying the tasks
        LoadAppSettingsOnce();

        AppSettings.Instance.AppWindowSettings.PropertyChanged += CommonSettings_PropertyChanged;

        Mediator.Register(OnUpdateMainPage, ViewModelMessages.UpdateMainPage);
    }

    private SessionSettings SessionSettings => AppSettings.Instance.SessionSettings;

    /// <summary>
    /// True if the side menu should be shown
    /// </summary>
    public bool SideMenuVisible { get; set; }

    /// <summary>
    /// The overlay panel content page
    /// </summary>
    public ApplicationPage OverlayPage { get; private set; }

    /// <summary>
    /// The view model to use for the current overlay page when the OverlayPage changes
    /// NOTE: This is not a live up-to-date view model of the current page
    ///       it is simply used to set the view model of the current page
    ///       at the time it changes
    /// </summary>
    public IBaseViewModel OverlayPageViewModel { get; set; }

    /// <summary>
    /// True if the overlay page should be shown
    /// </summary>
    public bool OverlayPageVisible { get; set; }

    /// <summary>
    /// The database location.
    /// </summary>
    public string DatabaseLocation => DataAccess.DatabasePath;

    /// <summary>
    /// The value change triggers the animation
    /// </summary>
    public bool SaveIconVisible { get; set; }

    // TODO: REMOVE
    private void OnUpdateMainPage(object obj)
    {
        UpdateMainPage();
    }

    /// <summary>
    /// Updates the Main Page based on what is stored in the application settings for active category or active note.
    /// </summary>
    public void UpdateMainPage()
    {
        if (SessionSettings.ActiveCategoryId == CommonConstants.RecycleBinCategoryId)
        {
            _mainPageNavigationService.NavigateTo<IRecycleBinPage>();
        }
        else if (SessionSettings.ActiveCategoryId != -1)
        {
            // The ActiveCategoryId and the ActiveNoteId must be mutually exclusive,
            // meaning that one or the other is set to -1 at all times, but never both at once.
            _mainPageNavigationService.NavigateTo<ITaskPage>();
        }
        else
        {
            _mainPageNavigationService.NavigateTo<INoteEditorPage>();
        }
    }

    /// <summary>
    /// Updates the SideMenu Page based on what is stored in the application settings for active category or active note.
    /// </summary>
    public void UpdateSideMenuPage()
    {
        // The ActiveCategoryId and the ActiveNoteId must be mutually exclusive,
        // meaning that one or the other is set to -1 at all times, but never both at once.
        if (SessionSettings.ActiveCategoryId != -1)
        {
            _sideMenuPageNavigationService.NavigateTo<ICategoryListPage>();
        }
        else
        {
            _sideMenuPageNavigationService.NavigateTo<INoteListPage>();
        }
    }

    public void OpenOverlayPage(ApplicationPage page, TaskViewModel task)
    {
        BaseViewModel viewModel = null;

        switch (page)
        {
            case ApplicationPage.TaskReminder:
            viewModel = new TaskReminderPageViewModel(_context, task);
            break;
            case ApplicationPage.ReminderEditor:
            viewModel = new ReminderEditorPageViewModel(_context, task);
            break;
            case ApplicationPage.Notification:
            viewModel = new NotificationPageViewModel(task);
            break;
            default:
            throw new ApplicationException("Invalid page.");
        }

        SideMenuVisible = false;

        GoToOverlayPage(page, true, viewModel);
        IoC.OverlayPageService.OverlayBackgroundVisible = true;
    }

    /// <summary>
    /// Closes the overlay page and disposes its viewModel
    /// </summary>
    public void CloseOverlayPage()
    {
        OverlayPage = ApplicationPage.Invalid;
        if (OverlayPageViewModel is IDisposable viewModel)
        {
            viewModel.Dispose();
        }
    }

    public void LoadAppSettingsOnce()
    {
        if (!_appSettingsLoadedFirstTime)
        {
            _appSettingsLoadedFirstTime = true;

            _settingsPopulator.UpdateAppSettingsFromDatabase(AppSettings.Instance);

            // Must be set to always check the registry on startup
            IoC.AutoRunService.RunAtStartup = AppSettings.Instance.AppWindowSettings.AutoStart;
        }
    }

    public void SaveApplicationSettings()
    {
        _settingsPopulator.UpdateDatabaseFromAppSettings(AppSettings.Instance);
    }

    private void CommonSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppWindowSettings.AutoStart))
        {
            IoC.AutoRunService.RunAtStartup = AppSettings.Instance.AppWindowSettings.AutoStart;
        }
    }

    /// <summary>
    /// Navigates the overlay page to the specified page
    /// </summary>
    /// <param name="page">The page to go to</param>
    /// <param name="visible">Shows the page if true</param>
    /// <param name="viewModel">The view model, if any, to set explicitly to the new page</param>
    private void GoToOverlayPage(ApplicationPage page, bool visible = true, IBaseViewModel viewModel = null)
    {
        // Always hide side menu if we are changing pages
        SideMenuVisible = false;

        // Set the view model
        OverlayPageViewModel = viewModel;

        // See if page has changed
        bool different = OverlayPage != page;

        // Set the current overlay page
        OverlayPage = page;

        // Show or hide the page
        OverlayPageVisible = visible;

        // If the page hasn't changed, fire off notification
        // So pages still update if just the view model has changed
        if (!different)
        {
            OnPropertyChanged(nameof(OverlayPage));
        }
    }
}