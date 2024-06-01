using Modules.Common.ViewModel;
using Modules.Settings.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TodoApp2.Core;
using TodoApp2.Persistence;
using TodoApp2.Services.Window;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Thickness = System.Windows.Thickness;

namespace TodoApp2;

/// <summary>
/// The View Model for the custom flat window
/// </summary>
public class MainWindowViewModel : BaseViewModel
{
    private IWindowService _windowService;
    private TrayIconModule _trayIconModule;
    private readonly ThemeManager _themeManager;

    private readonly AppViewModel _appViewModel;
    private readonly IAppContext _context;
    private readonly DragDropMediator _dragDropMediator;
    private readonly DispatcherTimer _timer;
    private bool _timeInitialized;
    private DateTime _initialTime;

    private AppWindowSettings AppWindowSettings => AppSettings.Instance.AppWindowSettings;
    private WindowSettings WindowSettings => AppSettings.Instance.WindowSettings;

    public ICommand MinimizeCommand { get; }
    public ICommand MaximizeCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }
    public ICommand ToggleSideMenuCommand { get; }

    public double WindowMinimumWidth { get; set; } = 220;
    public double WindowMinimumHeight { get; set; } = 200;
    public double ContentPadding { get; set; } = 0;

    // The padding of the inner content of the main window
    public Thickness InnerContentPadding => new Thickness(ContentPadding);

    // The size of the resize border around the window
    public int ResizeBorder => IsMaximized ? 0 : AppSettings.Instance.AppWindowSettings.ResizeBorderSize;

    // The size of the resize border around the window, taking into account the outer margin
    public Thickness ResizeBorderThickness => new Thickness(ResizeBorder);
    public bool IsMaximized => _windowService.IsMaximized;
    public bool IsMaximizedOrDocked => _windowService.IsMaximizedOrDocked;
    public long CurrentTime { get; set; }

    // AppWindowSettings.RoundedWindowCorners and this property both must be true for the rounded corners to work
    public bool IsRoundedCornersAllowed => _windowService.IsRoundedCornersAllowed;

    public bool IsTrayIconEnabled
    {
        get => _trayIconModule.IsEnabled;
        set => _trayIconModule.IsEnabled = value;
    }

    #region Workaround
    // WORKAROUND properties for MultiBinding bug
    // See: https://stackoverflow.com/questions/22536645/what-hardware-platform-difference-could-cause-an-xaml-wpf-multibinding-to-checkb
    public double MyWidth { get; set; }
    public double MyHeight { get; set; }
    public int OuterMargin => 2 * AppSettings.Instance.AppWindowSettings.ResizeBorderSize;
    public Rect ClipRect => new Rect(0, 0, MyWidth, MyHeight);
    public Rect OuterClipRect => new Rect(0, 0, MyWidth + OuterMargin, MyHeight + OuterMargin);

    #endregion Workaround

    public MainWindowViewModel(AppViewModel applicationViewModel, IAppContext context, IWindowService windowService)
    {
        ArgumentNullException.ThrowIfNull(applicationViewModel);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(windowService);

        _windowService = windowService;
        _appViewModel = applicationViewModel;
        _context = context;
        AppSettings.Instance.AppWindowSettings.PropertyChanged += CommonSettings_PropertyChanged;

        _themeManager = new ThemeManager();

        _windowService.Deactivated += (s, e) => _windowService.Topmost = AppWindowSettings.AlwaysOnTop;
        _windowService.Resized += (s, e) =>
        {
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(IsMaximized));
            OnPropertyChanged(nameof(IsMaximizedOrDocked));
        };
        _windowService.Loaded += OnWindowLoaded;
        _windowService.Closing += OnWindowClosing;
        _windowService.Closed += (s, e) => _context.Dispose();
        _windowService.RoundedCornersChanged += (s, e) => OnPropertyChanged(nameof(IsRoundedCornersAllowed));

        _trayIconModule = new TrayIconModule(_windowService);
        _trayIconModule.IsEnabled = AppWindowSettings.ExitToTray;

        // Create commands
        MinimizeCommand = new RelayCommand(() => _windowService.Minimize());
        MaximizeCommand = new RelayCommand(() => _windowService.Maximize());
        CloseCommand = new RelayCommand(CloseWindow);
        UndoCommand = new RelayCommand(() => { IoC.UndoManager.Undo(); });
        RedoCommand = new RelayCommand(() => { IoC.UndoManager.Redo(); });
        ToggleSideMenuCommand = new RelayCommand(ToggleSideMenu);

        // Listen out for requests to flash the application window
        Mediator.Register(OnWindowFlashRequested, ViewModelMessages.WindowFlashRequested);

        _appViewModel.UpdateMainPage();
        _appViewModel.UpdateSideMenuPage();

        _dragDropMediator = new DragDropMediator();

        _timer = new DispatcherTimer(DispatcherPriority.Send) { Interval = new TimeSpan(0, 0, 0, 0, 10) };
        CurrentTime = DateTime.Now.Ticks;
        _timer.Tick += TimerOnTickInitializer;
        _timer.Start();
    }

    private void TimerOnTickInitializer(object sender, EventArgs e)
    {
        if (!_timeInitialized)
        {
            _timeInitialized = true;
            _initialTime = DateTime.Now;
        }
        else
        {

            if (_initialTime.Second != DateTime.Now.Second)
            {
                // Accuracy set to 10 ms. Now switch to tick every seconds.
                _timer.Tick -= TimerOnTickInitializer;
                CurrentTime = DateTime.Now.Ticks;

                _timer.Interval = new TimeSpan(0, 0, 1);
                _timer.Tick += TimerOnTick;
            }
        }
    }

    private void TimerOnTick(object sender, EventArgs e)
    {
        CurrentTime = DateTime.Now.Ticks;
    }

    private void CommonSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppWindowSettings.ExitToTray))
        {
            _trayIconModule.IsEnabled = AppWindowSettings.ExitToTray;
        }
    }

    public void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            if (e.Delta > 0)
            {
                ZoomIn();
            }
            else if (e.Delta < 0)
            {
                ZoomOut();
            }
        }
    }

    // Global hotkeys for the window
    public void OnKeyDown(object sender, KeyEventArgs e)
    {
        Key key = e.Key;

        if (key == Key.Escape) IoC.OneEditorOpenService.EditMode(null);

        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        {
            // Ctrl + Z, Ctrl + Y
            if (key == Key.Z)
            {
                IoC.UndoManager.Undo();
            }
            else if (key == Key.Y)
            {
                IoC.UndoManager.Redo();
            }
            else if (key == Key.Subtract)
            {
                UIScaler.Instance.ZoomOut();
            }
            else if (key == Key.Add)
            {
                UIScaler.Instance.ZoomIn();
            }
            else if (key == Key.E)
            {
                // Ctrl + E
                // Set focus on task page bottom text editor
                Mediator.NotifyClients(ViewModelMessages.FocusBottomTextEditor);
            }

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                // Ctrl + Shift + J, Ctrl + Shift + L
                if (key == Key.J || key == Key.L)
                {
                    // TODO: think about it
                }
            }
        }
    }

    /// <summary>
    /// Used when 2 app instances communicate with each other, 
    /// to show the main window.
    /// </summary>
    public void ShowWindowRequested()
    {
        _trayIconModule.ShowWindow();
    }

    private void ToggleSideMenu()
    {
        Mediator.NotifyClients(ViewModelMessages.SideMenuButtonClicked);
    }

    private void ZoomOut()
    {
        IoC.UIScaler.ZoomOut();
    }

    private void ZoomIn()
    {
        IoC.UIScaler.ZoomIn();
    }

    private void CloseWindow()
    {
        if (_trayIconModule.IsEnabled)
        {
            _trayIconModule.MinimizeToTray();
        }
        else
        {
            _trayIconModule.Dispose();
            _windowService.Close();
        }
    }

    private void OnWindowFlashRequested(object obj)
    {
        bool playSound = (bool)obj;

        _windowService.FlashWindow(AppWindowSettings.AlwaysOnTop);

        // Play notification sound
        if (playSound)
        {
            WindowsEventSoundPlayer.PlayNotificationSound(EventSounds.MailBeep);
        }
    }

    private void OnWindowLoaded(object sender, EventArgs e)
    {
        // When the window finished loading,
        // load the settings from the database
        _appViewModel.LoadAppSettingsOnce();

        var left = WindowSettings.Left;
        var top = WindowSettings.Top;
        var width = WindowSettings.Width;
        var height = WindowSettings.Height;

        bool outOfBounds =
            (left <= SystemParameters.VirtualScreenLeft - width) ||
            (top <= SystemParameters.VirtualScreenTop - height) ||
            (SystemParameters.VirtualScreenLeft +
             SystemParameters.VirtualScreenWidth <= left) ||
            (SystemParameters.VirtualScreenTop +
             SystemParameters.VirtualScreenHeight <= top);

        // Check whether the last saved window position is visible on any screen or not
        // Restore the window position and size only if it is visible.
        // Note: If the restored window state would not be visible,
        //       the default window position is at center of screen
        if (!outOfBounds)
        {
            // Restore saved position and size
            _windowService.SetPosition(WindowSettings.Left, WindowSettings.Top);
            _windowService.SetSize(WindowSettings.Width, WindowSettings.Height);
        }
    }

    private void OnWindowClosing(object sender, EventArgs e)
    {
        WindowSettings.Left = (int)_windowService.Left;
        WindowSettings.Top = (int)_windowService.Top;

        if (!_windowService.IsMinimized)
        {
            // Only save the window size when the window is NOT minimized,
            // because the window size is invalid in that case.
            WindowSettings.Width = (int)_windowService.Width;
            WindowSettings.Height = (int)_windowService.Height;
        }

        _appViewModel.SaveApplicationSettings();
    }
}
