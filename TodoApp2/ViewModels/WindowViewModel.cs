using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TodoApp2.Core;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Thickness = System.Windows.Thickness;

namespace TodoApp2
{
    /// <summary>
    /// The View Model for the custom flat window
    /// </summary>
    public class WindowViewModel : BaseViewModel
    {
        private const string s_DateTimeFormatString = "yyyy-MM-dd HH:mm";
        private const int s_ResizeBorderSize = 9;

        private readonly Window m_Window;
        private readonly WindowResizer m_Resizer;
        private readonly TrayIconModule m_TrayIconModule;
        private readonly ThemeManager m_ThemeManager;

        private readonly AppViewModel m_AppViewModel;
        private readonly IDatabase m_Database;
        private readonly DragDropMediator m_DragDropMediator;
        private readonly DispatcherTimer m_Timer;
        private bool m_Closing;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition m_DockPosition = WindowDockPosition.Undocked;

        public ApplicationSettings ApplicationSettings => m_AppViewModel.ApplicationSettings;

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
        public int ResizeBorder => m_Window.WindowState == WindowState.Maximized ? 0 : s_ResizeBorderSize;

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
        /// <see cref="ApplicationSettings.RoundedWindowCorners"/> and this property both must be true 
        /// for the rounded corners to work.
        /// </summary>
        public bool IsRoundedCornersAllowed { get; set; }

        public string CurrentTime { get; set; }

        public bool IsTrayIconEnabled
        {
            get => m_TrayIconModule.IsEnabled;
            set => m_TrayIconModule.IsEnabled = value;
        }

        #region Workaround
        // WORKAROUND properties for MultiBinding bug
        // See: https://stackoverflow.com/questions/22536645/what-hardware-platform-difference-could-cause-an-xaml-wpf-multibinding-to-checkb
        public double MyWidth { get; set; }
        public double MyHeight { get; set; }
        public int OuterMargin { get; set; } = 2 * s_ResizeBorderSize;
        public Rect ClipRect => new Rect(0, 0, MyWidth, MyHeight);
        public Rect OuterClipRect => new Rect(0, 0, MyWidth + OuterMargin, MyHeight + OuterMargin);
        #endregion Workaround

        public WindowViewModel(Window window, AppViewModel applicationViewModel, IDatabase database)
        {
            m_Window = window;
            m_AppViewModel = applicationViewModel;
            m_Database = database;
            m_AppViewModel.ApplicationSettings.PropertyChanged += OnAppSettingsPropertyChanged;

            m_ThemeManager = new ThemeManager();

            m_Window.Deactivated += OnWindowDeactivated;

            // Listen out for all properties that are affected by a resize
            m_Window.StateChanged += OnWindowStateChanged;

            // Restore the last saved position and size of the window
            m_Window.Loaded += OnWindowLoaded;

            // Save the window size and position into the database
            m_Window.Closing += OnWindowClosing;

            // Dispose the database at last
            m_Window.Closed += OnWindowClosed;

            // Create commands
            MinimizeCommand = new RelayCommand(() => m_Window.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => m_Window.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(CloseWindow);
            UndoCommand = new RelayCommand(() => { IoC.UndoManager.Undo(); });
            RedoCommand = new RelayCommand(() => { IoC.UndoManager.Redo(); });
            ToggleSideMenuCommand = new RelayCommand(ToggleSideMenu);

            // Fix window resize issue
            m_Resizer = new WindowResizer(m_Window);
            m_Resizer.WindowDockChanged += OnWindowDockChanged;
            m_Resizer.IsDockedChanged += OnIsDockedChanged;

            m_TrayIconModule = new TrayIconModule(m_Window);
            m_TrayIconModule.IsEnabled = ApplicationSettings.IsTrayIconEnabled;

            // At application start, the saved theme is loaded back
            LoadBackTheme();

            // Listen out for requests to flash the application window
            Mediator.Register(OnWindowFlashRequested, ViewModelMessages.WindowFlashRequested);
            Mediator.Register(OnThemeChangeRequested, ViewModelMessages.ThemeChangeRequested);
            Mediator.Register(OnNextThemeWithHotkeyRequested, ViewModelMessages.NextThemeWithHotkeyRequested);

            // Subscribe to the theme changed event to trigger app border update
            Mediator.Register(OnThemeChanged, ViewModelMessages.ThemeChanged);

            m_AppViewModel.UpdateMainPage();
            m_AppViewModel.UpdateSideMenuPage();

            m_DragDropMediator = new DragDropMediator();

            m_Timer = new DispatcherTimer(DispatcherPriority.Send) { Interval = new TimeSpan(0, 0, 1) };
            CurrentTime = DateTime.Now.ToString(s_DateTimeFormatString);
            m_Timer.Tick += TimerOnTick;
            m_Timer.Start();
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

                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    // Ctrl + Shift + J, Ctrl + Shift + L
                    if (key == Key.J || key == Key.L)
                    {
                        ChangeActiveTheme(key == Key.L);
                    }
                }
            }
        }

        private void ChangeActiveTheme(bool next)
        {
            int themeCount = m_ThemeManager.ThemeList.Count;
            int currentIndex = m_ThemeManager.ThemeList.IndexOf(ApplicationSettings.ActiveTheme);
            int indexOffset = next ? 1 : -1;
            int nextIndex = (currentIndex + indexOffset) % themeCount;
            nextIndex = nextIndex < 0 ? themeCount - 1 : nextIndex;
            ApplicationSettings.ActiveTheme = m_ThemeManager.ThemeList[nextIndex];
        }

        private void OnNextThemeWithHotkeyRequested(object obj)
        {
            ChangeActiveTheme(true);
        }

        /// <summary>
        /// Used when 2 app instances communicate with each other, 
        /// to show the main window.
        /// </summary>
        public void ShowWindowRequested()
        {
            m_TrayIconModule.ShowWindow();
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
            if (m_TrayIconModule.IsEnabled)
            {
                m_TrayIconModule.MinimizeToTray();
            }
            else
            {
                m_TrayIconModule.Dispose();
                m_Window.Close();
            }
        }

        private void OnAppSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ApplicationSettings.IsTrayIconEnabled))
            {
                m_TrayIconModule.IsEnabled = ApplicationSettings.IsTrayIconEnabled;
            }
        }

        private void OnWindowFlashRequested(object obj)
        {
            bool playSound = (bool)obj;

            if (!m_Window.IsVisible)
            {
                m_Window.Show();
            }

            if (m_Window.WindowState == WindowState.Minimized)
            {
                m_Window.WindowState = WindowState.Normal;
            }

            m_Window.Activate();
            m_Window.Topmost = true;  // important
            m_Window.Topmost = false; // important
            m_Window.Focus();         // important
            m_Window.Topmost = ApplicationSettings.IsAlwaysOnTop;

            // Flash the window 3 times
            m_Window.FlashWindow(3);

            // Play notification sound
            if (playSound)
            {
                WindowsEventSoundPlayer.PlayNotificationSound(EventSounds.MailBeep);
            }
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            // Dispose the database
            m_Database.Dispose();
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
            bool isDocked = IsDocked || m_DockPosition != WindowDockPosition.Undocked;
            IsRoundedCornersAllowed = !(isDocked || IsMaximized);
        }

        private void OnWindowDeactivated(object sender, EventArgs e)
        {
            if (!m_Closing)
            {
                Window window = (Window)sender;
                bool isAlwaysOnTop = ApplicationSettings.IsAlwaysOnTop;
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
            m_DockPosition = dockPosition;

            // Fire off resize events
            WindowResized();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // When the window finished loading,
            // load the settings from the database
            m_AppViewModel.LoadApplicationSettingsOnce();

            var left = ApplicationSettings.WindowLeftPos;
            var top = ApplicationSettings.WindowTopPos;
            var width = ApplicationSettings.WindowWidth;
            var height = ApplicationSettings.WindowHeight;

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
                m_Window.Left = ApplicationSettings.WindowLeftPos;
                m_Window.Top = ApplicationSettings.WindowTopPos;
                m_Window.Width = ApplicationSettings.WindowWidth;
                m_Window.Height = ApplicationSettings.WindowHeight;
            }

            UpdateRoundedCornersAllowed();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            ApplicationSettings.WindowLeftPos = (int)GetWindowLeft(m_Window);
            ApplicationSettings.WindowTopPos = (int)GetWindowTop(m_Window);

            // Don't save window size when the window is minimized,
            // because the window size is invalid in this case.
            if (m_Window.WindowState != WindowState.Minimized)
            {
                ApplicationSettings.WindowWidth = (int)m_Window.Width;
                ApplicationSettings.WindowHeight = (int)m_Window.Height;
            }

            m_AppViewModel.SaveApplicationSettings();
            m_Closing = true;
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString(s_DateTimeFormatString);
        }

        private void OnThemeChangeRequested(object obj)
        {
            ChangeToActiveTheme();
        }

        private void OnThemeChanged(object obj)
        {
            // Trigger ui refresh for some properties that don't refresh automatically
            ApplicationSettings.TriggerUpdate(nameof(ApplicationSettings.AppBorderColor));
            ApplicationSettings.TriggerUpdate(nameof(ApplicationSettings.TitleColor));
        }

        private void ChangeToActiveTheme()
        {
            if (ApplicationSettings.ActiveTheme != ThemeManager.CurrentTheme)
            {
                m_ThemeManager.ChangeToTheme(ThemeManager.CurrentTheme, ApplicationSettings.ActiveTheme);
            }
        }

        /// <summary>
        /// Loads the initial theme that was saved in the database
        /// </summary>
        private void LoadBackTheme()
        {
            // Theme.Darker is the default, it is always the current theme at application start
            m_ThemeManager.ChangeToTheme(Theme.ExtraDark, ApplicationSettings.ActiveTheme);
        }

        /// <summary>
        /// Gets the current mouse position on the screen
        /// </summary>
        /// <returns></returns>
        private Point GetMousePosition()
        {
            // Position of the mouse relative to the window
            var position = Mouse.GetPosition(m_Window);

            // Adds the window position so its a "ToScreen"
            return new Point(position.X + m_Window.Left, position.Y + m_Window.Top);
        }

        /// <summary>
        /// If the window resizes to a special position (docked or maximized)
        /// this will update all required property change events to set the borders and radius values
        /// </summary>
        private void WindowResized()
        {
            IsMaximized = m_Window.WindowState == WindowState.Maximized;
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