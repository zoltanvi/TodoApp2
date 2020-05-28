using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TodoApp2.Core.Helpers;

namespace TodoApp2.Core
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

        private ClientDatabase Database => IoC.ClientDatabase;
        #endregion

        #region Commands

        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand NavigatorCommand { get; set; }

        public ICommand CloseOverlayCommand { get; set; }

        #endregion

        #region Public Properties

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

        public bool OverlayBackgroundVisible { get; set; }

        #endregion

        private ApplicationViewModel Application => IoC.Application;
        private ApplicationSettings ApplicationSettings => IoC.Application.ApplicationSettings;

        #region Constructors

        public WindowViewModel(Window window)
        {
            m_Window = window;

            // Listen out for all properties that are affected by a resize
            m_Window.StateChanged += OnWindowStateChanged;

            // Restore the last saved position and size of the window
            m_Window.Loaded += OnWindowOnLoaded;

            // Save the window size and position into the database
            m_Window.Closing += WindowOnClosing;

            // Dispose the database at last
            m_Window.Closed += WindowOnClosed;

            // Create commands
            MinimizeCommand = new RelayCommand(() => m_Window.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => m_Window.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(() => m_Window.Close());
            NavigatorCommand = new RelayCommand(OpenCloseNavigator);
            CloseOverlayCommand = new RelayCommand(CloseOverlay);

            // Fix window resize issue
            m_Resizer = new WindowResizer(m_Window);

            m_Resizer.WindowDockChanged += OnWindowDockChanged;
            m_Resizer.IsDockedChanged += ResizerOnIsDockedChanged;

            // Subscribe to the open reminder event to open the reminder panel
            Mediator.Instance.Register(OnOpenReminder, ViewModelMessages.OpenReminder);

            // Subscribe to the category changed event to turn off the overlay background
            Mediator.Instance.Register(OnCategoryChanged, ViewModelMessages.CategoryChanged);



            #region ONLY FOR TESTING

            IoC.ReminderTaskScheduler.ScheduledTask = ScheduledTask;

            DateTime current = DateTime.Now;

            for (int i = 0; i < 20; i++)
            {
                current = current.AddSeconds(5);
                IoC.ReminderTaskScheduler.Schedule(current, i);
            }

            #endregion ONLY FOR TESTING
        }

        #region ONLY FOR TESTING

        private async void ScheduledTask(int obj)
        {
            await Task.Run(() =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Window owner = System.Windows.Application.Current.MainWindow;

                    // Use owner here - it must be used on the UI thread as well..
                    owner.FlashWindow(5);
                });
            });
        }

        #endregion ONLY FOR TESTING

        private void OnOpenReminder(object obj)
        {
            OverlayBackgroundVisible = true;
            // TODO: open reminder
        }

        private void OnCategoryChanged(object obj)
        {
            OverlayBackgroundVisible = false;
        }

        private void CloseOverlay()
        {
            OverlayBackgroundVisible = false;

            // Notify all listeners about the background close
            Mediator.Instance.NotifyClients(ViewModelMessages.OverlayBackgroundClosed, false);

            Application.SideMenuVisible = false;
        }

        private void WindowOnClosed(object sender, EventArgs e)
        {
            // Dispose the database
            IoC.Get<ClientDatabase>().Dispose();
        }

        private void OpenCloseNavigator()
        {
            Application.SideMenuVisible ^= true;
            OverlayBackgroundVisible = Application.SideMenuVisible;
        }

        private void ResizerOnIsDockedChanged(object sender, DockChangeEventArgs e)
        {
            IsDocked = e.IsDocked;
            WindowResized();
        }

        #endregion

        #region EventHandlers

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

        private void OnWindowOnLoaded(object sender, RoutedEventArgs e)
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

        private void WindowOnClosing(object sender, CancelEventArgs e)
        {
            ApplicationSettings.WindowLeftPos = (int)GetWindowLeft(m_Window);
            ApplicationSettings.WindowTopPos = (int)GetWindowTop(m_Window);
            ApplicationSettings.WindowWidth = (int)m_Window.Width;
            ApplicationSettings.WindowHeight = (int)m_Window.Height;

            Application.SaveApplicationSettings();
        }

        #endregion

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

        #endregion
    }
}
