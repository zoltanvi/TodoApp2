using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TodoApp2.Core;

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
        private int m_OuterMarginSize = 2;
        private int m_WindowRadius = 0;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition m_DockPosition = WindowDockPosition.Undocked;

        private ApplicationViewModel Application => IoC.Application;
        private ApplicationSettings ApplicationSettings => IoC.Application.ApplicationSettings;
        private OverlayPageService OverlayPageService => IoC.OverlayPageService;

        #endregion Private Fields

        #region Window handling commands

        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand CloseCommand { get; }

        #endregion Window handling commands

        #region Window settings

        public double WindowMinimumWidth { get; set; } = 320;
        public double WindowMinimumHeight { get; set; } = 400;
        public double ContentPadding { get; set; } = 0;

        /// <summary>
        /// The padding of the inner content of the main window
        /// </summary>
        public Thickness InnerContentPadding => new Thickness(ContentPadding);

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public int ResizeBorder => m_Window.WindowState == WindowState.Maximized ? 0 : 4;

        /// <summary>
        /// The size of the resize border around the window, taking into account the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness => new Thickness(ResizeBorder);

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public int OuterMarginSize
        {
            // If it is maximized or docked, no border
            get => Borderless ? 0 : m_OuterMarginSize;
            set => m_OuterMarginSize = value;
        }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public Thickness OuterMarginThickness => new Thickness(OuterMarginSize);

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public int WindowRadius
        {
            // If it is maximized or docked, no border
            get => Borderless ? 0 : m_WindowRadius;
            set => m_WindowRadius = value;
        }

        /// <summary>
        /// True if the window should be borderless because it is docked or maximized
        /// </summary>
        public bool Borderless => IsDocked || m_Window.WindowState == WindowState.Maximized || m_DockPosition != WindowDockPosition.Undocked;

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public CornerRadius WindowCornerRadius => new CornerRadius(WindowRadius);

        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public int TitleBarHeight { get; set; } = 35;

        /// <summary>
        /// The height of the TitleBar of the window
        /// </summary>
        public GridLength TitleBarGridHeight => new GridLength(TitleBarHeight);

        public bool IsDocked { get; set; }

        #endregion Window settings

        #region Constructors

        public WindowViewModel(Window window)
        {
            m_Window = window;

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

            // Listen out for requests to flash the application window
            Mediator.Instance.Register(OnWindowFlashRequested, ViewModelMessages.WindowFlashRequested);
        }

        #endregion Constructors

        #region EventHandlers

        private void OnWindowFlashRequested(object obj)
        {
            bool playSound = (bool)obj;

            // Flash the window 3 times
            m_Window.FlashWindow(3);

            // Play notification sound
            if (playSound)
            {
                WindowsEventSoundPlayer.PlayNotificationSound(EventSounds.MailBeep);
            }

            // Bring to front
            m_Window.Topmost = true;
            m_Window.Activate();
            m_Window.Topmost = Application.IsAlwaysOnTop;
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            // Dispose the database
            IoC.ClientDatabase.Dispose();
        }

        private void OnIsDockedChanged(object sender, DockChangeEventArgs e)
        {
            IsDocked = e.IsDocked;
            WindowResized();
        }

        private void OnWindowDeactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = Application.IsAlwaysOnTop;
        }

        private void OnWindowStateChanged(object sender, EventArgs e)
        {
            // Fire off events for all properties that are affected by a resize
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(OuterMarginSize));
            OnPropertyChanged(nameof(OuterMarginThickness));
            OnPropertyChanged(nameof(WindowCornerRadius));
            OnPropertyChanged(nameof(WindowRadius));
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
            Application.LoadApplicationSettingsOnce();

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

            Application.SaveApplicationSettings();
        }

        #endregion EventHandlers

        #region Private helpers

    
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
            // Fire off events for all properties that are affected by a resize
            OnPropertyChanged(nameof(Borderless));
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(OuterMarginSize));
            OnPropertyChanged(nameof(OuterMarginThickness));
            OnPropertyChanged(nameof(WindowRadius));
            OnPropertyChanged(nameof(WindowCornerRadius));
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