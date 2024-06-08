using Modules.Common.DataModels;
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
    private readonly IOverlayPageNavigationService _overlayPageNavigationService;

    public AppViewModel(
        IAppContext context,
        UIScaler uiScaler,
        IAppSettingsService settingsPopulator,
        IServiceProvider serviceProvider,
        IMainPageNavigationService mainPageNavigationService,
        ISideMenuPageNavigationService sideMenuPageNavigationService,
        IOverlayPageNavigationService overlayPageNavigationService)
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
        _overlayPageNavigationService = overlayPageNavigationService;

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

    public void OpenOverlayPage<T>(TaskViewModel task) where T : class, IPage
    {
        IoC.OverlayPageService.OverlayBackgroundVisible = true;
        IoC.OverlayPageService.Task = task;

        _overlayPageNavigationService.NavigateTo<T>();

        SideMenuVisible = false;
        OverlayPageVisible = true;
    }

    /// <summary>
    /// Closes the overlay page and disposes its viewModel
    /// </summary>
    public void CloseOverlayPage()
    {
        _overlayPageNavigationService.NavigateTo<IEmptyPage>();
        IoC.OverlayPageService.Task = null;
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
}