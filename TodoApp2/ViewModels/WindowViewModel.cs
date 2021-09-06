using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TodoApp2.Core;
using Thickness = System.Windows.Thickness;

namespace TodoApp2
{
    /// <summary>
    /// The View Model for the custom flat window
    /// </summary>
    public class WindowViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly Window m_Window;
        private readonly WindowResizer m_Resizer;
        private readonly ThemeManager m_ThemeManager;

        private readonly TaskListService m_TaskListService;
        private readonly ApplicationViewModel m_ApplicationViewModel;
        private readonly CategoryListService m_CategoryListService;
        private readonly IDatabase m_Database;
        private readonly DragDropMediator m_DragDropMediator;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition m_DockPosition = WindowDockPosition.Undocked;

        private ApplicationSettings ApplicationSettings => m_ApplicationViewModel.ApplicationSettings;

        #endregion Private Fields

        #region Window handling commands

        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand CloseCommand { get; }

        #endregion Window handling commands

        #region Window settings

        public double WindowMinimumWidth { get; set; } = 300;
        public double WindowMinimumHeight { get; set; } = 215;
        public double ContentPadding { get; set; } = 0;

        /// <summary>
        /// The padding of the inner content of the main window
        /// </summary>
        public Thickness InnerContentPadding => new Thickness(ContentPadding);

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public int ResizeBorder => m_Window.WindowState == WindowState.Maximized ? 0 : 5;

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
        public string CurrentTime { get; set; }

        #endregion Window settings
        
        #region Workaround
        // WORKAROUND properties for MultiBinding bug
        // See: https://stackoverflow.com/questions/22536645/what-hardware-platform-difference-could-cause-an-xaml-wpf-multibinding-to-checkb
        public double MyWidth { get; set; }
        public double MyHeight { get; set; }
        public Rect ClipRect => new Rect(0, 0, MyWidth, MyHeight);
        #endregion Workaround

        #region Constructors

        public WindowViewModel(Window window, ApplicationViewModel applicationViewModel,
            TaskListService taskListService, CategoryListService categoryListService, IDatabase database)
        {
            m_Window = window;
            m_ApplicationViewModel = applicationViewModel;
            m_TaskListService = taskListService;
            m_CategoryListService = categoryListService;
            m_Database = database;

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
            CloseCommand = new RelayCommand(() => m_Window.Close());

            // Fix window resize issue
            m_Resizer = new WindowResizer(m_Window);

            m_Resizer.WindowDockChanged += OnWindowDockChanged;
            m_Resizer.IsDockedChanged += OnIsDockedChanged;

            // At application start, the saved theme is loaded back
            LoadBackTheme();

            // Listen out for requests to flash the application window
            Mediator.Register(OnWindowFlashRequested, ViewModelMessages.WindowFlashRequested);
            Mediator.Register(OnThemeChangeRequested, ViewModelMessages.ThemeChangeRequested);

            // Set initial main and side menu pages
            m_ApplicationViewModel.CurrentPage = ApplicationPage.Task;
            m_ApplicationViewModel.SideMenuPage = ApplicationPage.Category;

            m_DragDropMediator = new DragDropMediator();

            Timer = new DispatcherTimer(DispatcherPriority.Send) { Interval = new TimeSpan(0, 0, 10) };
            CurrentTime = DateTime.Now.ToString(DateTimeFormatString);
            Timer.Tick += TimerOnTick;
            Timer.Start();
        }

        private DispatcherTimer Timer;
        private const string DateTimeFormatString = "yyyy-MM-dd HH:mm";

        private void TimerOnTick(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString(DateTimeFormatString);
        }

        private void OnThemeChangeRequested(object obj)
        {
            if (ApplicationSettings.ActiveTheme != m_ThemeManager.CurrentTheme)
            {
                m_ThemeManager.ChangeToTheme(m_ThemeManager.CurrentTheme, ApplicationSettings.ActiveTheme);
                Mediator.NotifyClients(ViewModelMessages.ThemeChanged);
            }
        }

        #endregion Constructors

        #region EventHandlers

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
            UpdateWindowCornerRadius();
            WindowResized();
        }

        private void UpdateWindowCornerRadius()
        {
            bool isDocked = IsDocked || m_DockPosition != WindowDockPosition.Undocked;
            bool roundedCornersAllowed = !(isDocked || IsMaximized);
            bool isRoundedCornersOn = ApplicationSettings.RoundedWindowCorners;

            ApplicationSettings.WindowCornerRadius = roundedCornersAllowed && isRoundedCornersOn ? 8 : 0;
        }

        private void OnWindowDeactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            bool isAlwaysOnTop = ApplicationSettings.IsAlwaysOnTop;
            window.Topmost = isAlwaysOnTop;
            if (isAlwaysOnTop)
            {
                window.Show();
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
            m_ApplicationViewModel.LoadApplicationSettingsOnce();

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
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            ApplicationSettings.WindowLeftPos = (int)GetWindowLeft(m_Window);
            ApplicationSettings.WindowTopPos = (int)GetWindowTop(m_Window);
            ApplicationSettings.WindowWidth = (int)m_Window.Width;
            ApplicationSettings.WindowHeight = (int)m_Window.Height;

            m_ApplicationViewModel.SaveApplicationSettings();
        }

        #endregion EventHandlers

        #region Private helpers

        /// <summary>
        /// Loads the initial theme that was saved in the database
        /// </summary>
        private void LoadBackTheme()
        {
            // Theme.Darker is the default, it is always the current theme at application start
            m_ThemeManager.ChangeToTheme(Theme.Dark_1, ApplicationSettings.ActiveTheme);
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
            UpdateWindowCornerRadius();

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

        #endregion Private helpers
    }
}