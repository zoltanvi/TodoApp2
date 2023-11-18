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
    public class WindowViewModel : BaseViewModel
    {
        private const int ResizeBorderSize = 9;

        private readonly Window _window;
        private readonly WindowResizer _resizer;
        private readonly TrayIconModule _trayIconModule;
        private readonly ThemeManager _themeManager;

        private readonly AppViewModel _appViewModel;
        private readonly IAppContext _context;
        private readonly DragDropMediator _dragDropMediator;
        private readonly DispatcherTimer _timer;
        private bool _closing;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition _dockPosition = WindowDockPosition.Undocked;

        private CommonSettings CommonSettings => IoC.AppSettings.CommonSettings;
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
        public int ResizeBorder => _window.WindowState == WindowState.Maximized ? 0 : ResizeBorderSize;

        /// <summary>
        /// The size of the resize border around the window, taking into account the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness => new Thickness(ResizeBorder);

        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public int TitleBarHeight { get; set; } = 32;

        /// <summary>
        /// The height of the TitleBar of the window
        /// </summary>
        public GridLength TitleBarGridHeight => new GridLength(TitleBarHeight);

        public bool IsDocked { get; set; }
        public bool IsMaximized { get; set; }
        public bool IsMaximizedOrDocked { get; set; }

        /// <summary>
        /// <see cref="CommonSettings.RoundedWindowCorners"/> and this property both must be true 
        /// for the rounded corners to work.
        /// </summary>
        public bool IsRoundedCornersAllowed { get; set; }

        public long CurrentTime { get; set; }

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
        public int OuterMargin { get; set; } = 2 * ResizeBorderSize;
        public Rect ClipRect => new Rect(0, 0, MyWidth, MyHeight);
        public Rect OuterClipRect => new Rect(0, 0, MyWidth + OuterMargin, MyHeight + OuterMargin);
        #endregion Workaround

        public WindowViewModel(Window window, AppViewModel applicationViewModel, IAppContext context)
        {
            _window = window;
            _appViewModel = applicationViewModel;
            _context = context;
            IoC.AppSettings.CommonSettings.PropertyChanged += CommonSettings_PropertyChanged;

            _themeManager = new ThemeManager();

            _window.Deactivated += OnWindowDeactivated;

            // Listen out for all properties that are affected by a resize
            _window.StateChanged += OnWindowStateChanged;

            // Restore the last saved position and size of the window
            _window.Loaded += OnWindowLoaded;

            // Save the window size and position into the database
            _window.Closing += OnWindowClosing;

            // Dispose the database at last
            _window.Closed += OnWindowClosed;

            // Create commands
            MinimizeCommand = new RelayCommand(() => _window.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => _window.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(CloseWindow);
            UndoCommand = new RelayCommand(() => { IoC.UndoManager.Undo(); });
            RedoCommand = new RelayCommand(() => { IoC.UndoManager.Redo(); });
            ToggleSideMenuCommand = new RelayCommand(ToggleSideMenu);

            // Fix window resize issue
            _resizer = new WindowResizer(_window);
            _resizer.WindowDockChanged += OnWindowDockChanged;
            _resizer.IsDockedChanged += OnIsDockedChanged;

            _trayIconModule = new TrayIconModule(_window);
            _trayIconModule.IsEnabled = CommonSettings.ExitToTray;

            // Listen out for requests to flash the application window
            Mediator.Register(OnWindowFlashRequested, ViewModelMessages.WindowFlashRequested);

            _appViewModel.UpdateMainPage();
            _appViewModel.UpdateSideMenuPage();

            _dragDropMediator = new DragDropMediator();

            _timer = new DispatcherTimer(DispatcherPriority.Send) { Interval = new TimeSpan(0, 0, 1) };
            CurrentTime = DateTime.Now.Ticks;
            _timer.Tick += TimerOnTick;
            _timer.Start();
        }

        private void CommonSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CommonSettings.ExitToTray))
            {
                _trayIconModule.IsEnabled = CommonSettings.ExitToTray;
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

            if(key == Key.Escape) IoC.OneEditorOpenService.EditMode(null);

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
                _window.Close();
            }
        }

        private void OnWindowFlashRequested(object obj)
        {
            bool playSound = (bool)obj;

            if (!_window.IsVisible)
            {
                _window.Show();
            }

            if (_window.WindowState == WindowState.Minimized)
            {
                _window.WindowState = WindowState.Normal;
            }

            _window.Activate();
            _window.Topmost = true;  // important
            _window.Topmost = false; // important
            _window.Focus();         // important
            _window.Topmost = CommonSettings.AlwaysOnTop;

            // Flash the window 3 times
            _window.FlashWindow(3);

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
                bool isAlwaysOnTop = CommonSettings.AlwaysOnTop;
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
                _window.Left = WindowSettings.Left;
                _window.Top = WindowSettings.Top;
                _window.Width = WindowSettings.Width;
                _window.Height = WindowSettings.Height;
            }

            UpdateRoundedCornersAllowed();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            WindowSettings.Left = (int)GetWindowLeft(_window);
            WindowSettings.Top = (int)GetWindowTop(_window);

            // Don't save window size when the window is minimized,
            // because the window size is invalid in this case.
            if (_window.WindowState != WindowState.Minimized)
            {
                WindowSettings.Width = (int)_window.Width;
                WindowSettings.Height = (int)_window.Height;
            }

            _appViewModel.SaveApplicationSettings();
            _closing = true;
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Gets the current mouse position on the screen
        /// </summary>
        /// <returns></returns>
        private Point GetMousePosition()
        {
            // Position of the mouse relative to the window
            var position = Mouse.GetPosition(_window);

            // Adds the window position so its a "ToScreen"
            return new Point(position.X + _window.Left, position.Y + _window.Top);
        }

        /// <summary>
        /// If the window resizes to a special position (docked or maximized)
        /// this will update all required property change events to set the borders and radius values
        /// </summary>
        private void WindowResized()
        {
            IsMaximized = _window.WindowState == WindowState.Maximized;
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