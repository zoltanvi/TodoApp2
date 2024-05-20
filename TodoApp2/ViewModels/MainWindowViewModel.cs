using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TodoApp2.Core;
using TodoApp2.Core.Extensions;
using TodoApp2.Persistence;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Thickness = System.Windows.Thickness;

namespace TodoApp2
{
    /// <summary>
    /// The View Model for the custom flat window
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        //private const int ResizeBorderSize = 9;
        private Window _window;
        public Window Window
        {
            get => _window;
            set
            {
                if (_window != null)
                {
                    throw new ApplicationException("Cannot set window more than once!");
                }

                _window = value;

                Window.Deactivated += OnWindowDeactivated;

                // Listen out for all properties that are affected by a resize
                Window.StateChanged += OnWindowStateChanged;

                // Restore the last saved position and size of the window
                Window.Loaded += OnWindowLoaded;

                // Save the window size and position into the database
                Window.Closing += OnWindowClosing;

                // Dispose the database at last
                Window.Closed += OnWindowClosed;

                // Fix window resize issue
                _resizer = new WindowResizer(Window);
                _resizer.WindowDockChanged += OnWindowDockChanged;
                _resizer.IsDockedChanged += OnIsDockedChanged;

                _trayIconModule = new TrayIconModule(Window);
                _trayIconModule.IsEnabled = AppWindowSettings.ExitToTray;
            }
        }

        private WindowResizer _resizer;
        private TrayIconModule _trayIconModule;
        private readonly ThemeManager _themeManager;

        private readonly AppViewModel _appViewModel;
        private readonly IAppContext _context;
        private readonly DragDropMediator _dragDropMediator;
        private readonly DispatcherTimer _timer;
        private bool _closing;
        private bool _timeInitialized;
        private DateTime _initialTime;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition _dockPosition = WindowDockPosition.Undocked;

        private AppWindowSettings AppWindowSettings => IoC.AppSettings.AppWindowSettings;
        private WindowSettings WindowSettings => IoC.AppSettings.WindowSettings;

        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand ToggleSideMenuCommand { get; }

        public double WindowMinimumWidth { get; set; } = 220;
        public double WindowMinimumHeight { get; set; } = 200;
        public double ContentPadding { get; set; } = 0;

        /// <summary>
        /// The padding of the inner content of the main window
        /// </summary>
        public Thickness InnerContentPadding => new Thickness(ContentPadding);

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public int ResizeBorder => Window.WindowState == WindowState.Maximized
            ? 0
            : IoC.AppSettings.AppWindowSettings.ResizeBorderSize;

        /// <summary>
        /// The size of the resize border around the window, taking into account the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness => new Thickness(ResizeBorder);

        public bool IsDocked { get; set; }
        public bool IsMaximized { get; set; }
        public bool IsMaximizedOrDocked { get; set; }

        /// <summary>
        /// <see cref="AppWindowSettings.RoundedWindowCorners"/> and this property both must be true 
        /// for the rounded corners to work.
        /// </summary>
        public bool IsRoundedCornersAllowed { get; set; }

        public long CurrentTime { get; set; }

        public bool IsTrayIconEnabled
        {
            get => _trayIconModule.IsEnabled;
            set => _trayIconModule.IsEnabled = value;
        }

        /// <summary>
        /// Represents whether the window is active or not.
        /// </summary>
        public bool Active { get; set; }

        #region Workaround
        // WORKAROUND properties for MultiBinding bug
        // See: https://stackoverflow.com/questions/22536645/what-hardware-platform-difference-could-cause-an-xaml-wpf-multibinding-to-checkb
        public double MyWidth { get; set; }
        public double MyHeight { get; set; }
        public int OuterMargin => 2 * IoC.AppSettings.AppWindowSettings.ResizeBorderSize;
        public Rect ClipRect => new Rect(0, 0, MyWidth, MyHeight);
        public Rect OuterClipRect => new Rect(0, 0, MyWidth + OuterMargin, MyHeight + OuterMargin);

        #endregion Workaround

        public MainWindowViewModel(AppViewModel applicationViewModel, IAppContext context)
        {
            _appViewModel = applicationViewModel;
            _context = context;
            IoC.AppSettings.AppWindowSettings.PropertyChanged += CommonSettings_PropertyChanged;

            _themeManager = new ThemeManager();

            //Window.Deactivated += OnWindowDeactivated;

            //// Listen out for all properties that are affected by a resize
            //Window.StateChanged += OnWindowStateChanged;

            //// Restore the last saved position and size of the window
            //Window.Loaded += OnWindowLoaded;

            //// Save the window size and position into the database
            //Window.Closing += OnWindowClosing;

            //// Dispose the database at last
            //Window.Closed += OnWindowClosed;

            // Create commands
            MinimizeCommand = new RelayCommand(() => Window.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => Window.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(CloseWindow);
            UndoCommand = new RelayCommand(() => { IoC.UndoManager.Undo(); });
            RedoCommand = new RelayCommand(() => { IoC.UndoManager.Redo(); });
            ToggleSideMenuCommand = new RelayCommand(ToggleSideMenu);

            //// Fix window resize issue
            //_resizer = new WindowResizer(Window);
            //_resizer.WindowDockChanged += OnWindowDockChanged;
            //_resizer.IsDockedChanged += OnIsDockedChanged;

            //_trayIconModule = new TrayIconModule(Window);
            //_trayIconModule.IsEnabled = AppWindowSettings.ExitToTray;

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
                    UIScaler.ZoomOut();
                }
                else if (key == Key.Add)
                {
                    UIScaler.ZoomIn();
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
                Window.Close();
            }
        }

        private void OnWindowFlashRequested(object obj)
        {
            bool playSound = (bool)obj;

            if (!Window.IsVisible)
            {
                Window.Show();
            }

            if (Window.WindowState == WindowState.Minimized)
            {
                Window.WindowState = WindowState.Normal;
            }

            Window.Activate();
            Window.Topmost = true;  // important
            Window.Topmost = false; // important
            Window.Focus();         // important
            Window.Topmost = AppWindowSettings.AlwaysOnTop;

            // Flash the window 3 times
            Window.FlashWindow(3);

            // Play notification sound
            if (playSound)
            {
                WindowsEventSoundPlayer.PlayNotificationSound(EventSounds.MailBeep);
            }
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            // Dispose the database
            _context.Dispose();
        }

        private void OnIsDockedChanged(object sender, DockChangeEventArgs e)
        {
            IsDocked = e.IsDocked;
            IsMaximizedOrDocked = IsMaximized || IsDocked;
            UpdateRoundedCornersAllowed();
            WindowResized();
        }

        private void UpdateRoundedCornersAllowed()
        {
            bool isDocked = IsDocked || _dockPosition != WindowDockPosition.Undocked;
            IsRoundedCornersAllowed = !(isDocked || IsMaximized);
        }

        private void OnWindowDeactivated(object sender, EventArgs e)
        {
            if (!_closing)
            {
                Window window = (Window)sender;
                bool isAlwaysOnTop = AppWindowSettings.AlwaysOnTop;
                window.Topmost = isAlwaysOnTop;
                if (isAlwaysOnTop)
                {
                    window.Show();
                }
            }
        }

        private void OnWindowStateChanged(object sender, EventArgs e)
        {
            // Fire off events for all properties that are affected by a resize
            OnPropertyChanged(nameof(ResizeBorderThickness));
            WindowResized();
        }

        private void OnWindowDockChanged(WindowDockPosition dockPosition)
        {
            // Store last position
            _dockPosition = dockPosition;

            // Fire off resize events
            WindowResized();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
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
                Window.Left = WindowSettings.Left;
                Window.Top = WindowSettings.Top;
                Window.Width = WindowSettings.Width;
                Window.Height = WindowSettings.Height;
            }

            UpdateRoundedCornersAllowed();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            WindowSettings.Left = (int)GetWindowLeft(Window);
            WindowSettings.Top = (int)GetWindowTop(Window);

            // Don't save window size when the window is minimized,
            // because the window size is invalid in this case.
            if (Window.WindowState != WindowState.Minimized)
            {
                WindowSettings.Width = (int)Window.Width;
                WindowSettings.Height = (int)Window.Height;
            }

            _appViewModel.SaveApplicationSettings();
            _closing = true;
        }

        /// <summary>
        /// If the window resizes to a special position (docked or maximized)
        /// this will update all required property change events to set the borders and radius values
        /// </summary>
        private void WindowResized()
        {
            IsMaximized = Window.WindowState == WindowState.Maximized;
            IsMaximizedOrDocked = IsMaximized || IsDocked;
            UpdateRoundedCornersAllowed();

            // Fire off events for all properties that are affected by a resize
            OnPropertyChanged(nameof(ResizeBorderThickness));
        }

        /// <summary>
        /// Returns the position of the window's left edge, in relation to the desktop.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>Returns the position of the left edge.</returns>
        private double GetWindowLeft(Window window)
        {
            // If the window is maximized, window.Left gives back incorrect value
            if (window.WindowState == WindowState.Maximized)
            {
                return window.GetFieldValue<double>("_actualLeft");
            }

            return window.Left;
        }

        /// <summary>
        /// Returns the position of the window's top edge, in relation to the desktop.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>Returns the position of the top edge.</returns>
        private double GetWindowTop(Window window)
        {
            // If the window is maximized, window.Top gives back incorrect value
            if (window.WindowState == WindowState.Maximized)
            {
                return window.GetFieldValue<double>("_actualTop");
            }

            return window.Top;
        }
    }
}