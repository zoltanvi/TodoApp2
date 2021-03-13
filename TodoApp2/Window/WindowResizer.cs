using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace TodoApp2
{
    public class DockChangeEventArgs : EventArgs
    {
        public DockChangeEventArgs(bool isDocked)
        {
            IsDocked = isDocked;
        }

        public bool IsDocked { get; }
    }

    /// <summary>
    /// The dock position of the window
    /// </summary>
    public enum WindowDockPosition
    {
        /// <summary>
        /// Not docked
        /// </summary>
        Undocked,

        /// <summary>
        /// Docked to the left of the screen
        /// </summary>
        Left,

        /// <summary>
        /// Docked to the right of the screen
        /// </summary>
        Right,
    }

    /// <summary>
    /// Fixes the issue with Windows of Style <see cref="WindowStyle.None"/> covering the taskbar
    /// </summary>
    public class WindowResizer
    {
        #region Private Members

        /// <summary>
        /// The window to handle the resizing for
        /// </summary>
        private Window m_Window;

        /// <summary>
        /// The last calculated available screen size
        /// </summary>
        private Rect m_ScreenSize = new Rect();

        /// <summary>
        /// How close to the edge the window has to be to be detected as at the edge of the screen
        /// </summary>
        private int m_EdgeTolerance = 2;

        /// <summary>
        /// The transform matrix used to convert WPF sizes to screen pixels
        /// </summary>
        private Matrix m_TransformToDevice;

        /// <summary>
        /// The last screen the window was on
        /// </summary>
        private IntPtr m_LastScreen;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition m_LastDock = WindowDockPosition.Undocked;

        private bool m_IsSnapped;

        private bool IsSnapped
        {
            get => m_IsSnapped;
            set
            {
                if (m_IsSnapped != value)
                {
                    m_IsSnapped = value;
                    IsDockedChanged?.Invoke(this, new DockChangeEventArgs(m_IsSnapped));
                }
            }
        }

        #endregion Private Members

        #region Dll Imports

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        [DllImport("user32")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #endregion Dll Imports

        #region Public Events

        /// <summary>
        /// Called when the window dock position changes
        /// </summary>
        public event Action<WindowDockPosition> WindowDockChanged = (dock) => { };

        public event EventHandler<DockChangeEventArgs> IsDockedChanged;

        #endregion Public Events

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window">The window to monitor and correctly maximize</param>
        /// <param name="adjustSize">The callback for the host to adjust the maximum available size if needed</param>
        public WindowResizer(Window window)
        {
            m_Window = window;

            // Create transform visual (for converting WPF size to pixel size)
            GetTransform();

            // Listen out for source initialized to setup
            m_Window.SourceInitialized += Window_SourceInitialized;

            // Monitor for edge docking
            m_Window.SizeChanged += Window_SizeChanged;
        }

        #endregion Constructor

        #region Initialize

        /// <summary>
        /// Gets the transform object used to convert WPF sizes to screen pixels
        /// </summary>
        private void GetTransform()
        {
            // Get the visual source
            var source = PresentationSource.FromVisual(m_Window);

            // Reset the transform to default
            m_TransformToDevice = default(Matrix);

            // If we cannot get the source, ignore
            if (source == null)
                return;

            // Otherwise, get the new transform object
            m_TransformToDevice = source.CompositionTarget.TransformToDevice;
        }

        /// <summary>
        /// Initialize and hook into the windows message pump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SourceInitialized(object sender, System.EventArgs e)
        {
            // Get the handle of this window
            var handle = (new WindowInteropHelper(m_Window)).Handle;
            var handleSource = HwndSource.FromHwnd(handle);

            // If not found, end
            if (handleSource == null)
                return;

            // Hook into it's Windows messages
            handleSource.AddHook(WindowProc);
        }

        #endregion Initialize

        #region Edge Docking

        /// <summary>
        /// Monitors for size changes and detects if the window has been docked (Aero snap) to an edge
        /// TODO: Aero snap is not working with this method! Workaround: WindowProc method case WM_SIZE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // We cannot find positioning until the window transform has been established
            if (m_TransformToDevice == default(Matrix))
                return;

            // Get the WPF size
            var size = e.NewSize;

            // Get window rectangle
            var top = m_Window.Top;
            var left = m_Window.Left;
            var bottom = top + size.Height;
            var right = left + m_Window.Width;

            // Get window position/size in device pixels
            var windowTopLeft = m_TransformToDevice.Transform(new Point(left, top));
            var windowBottomRight = m_TransformToDevice.Transform(new Point(right, bottom));

            // Check for edges docked
            var edgedTop = windowTopLeft.Y <= (m_ScreenSize.Top + m_EdgeTolerance);
            var edgedLeft = windowTopLeft.X <= (m_ScreenSize.Left + m_EdgeTolerance);
            var edgedBottom = windowBottomRight.Y >= (m_ScreenSize.Bottom - m_EdgeTolerance);
            var edgedRight = windowBottomRight.X >= (m_ScreenSize.Right - m_EdgeTolerance);

            // Get docked position
            var dock = WindowDockPosition.Undocked;

            // Left docking
            if (edgedTop && edgedBottom && edgedLeft)
                dock = WindowDockPosition.Left;
            else if (edgedTop && edgedBottom && edgedRight)
                dock = WindowDockPosition.Right;
            // None
            else
                dock = WindowDockPosition.Undocked;

            // If dock has changed
            if (dock != m_LastDock)
                // Inform listeners
                WindowDockChanged(dock);

            // Save last dock position
            m_LastDock = dock;
        }

        #endregion Edge Docking

        #region Windows Proc

        private const Int32 WM_GETMINMAXINFO = 0x0024;
        private const Int32 WM_SIZE = 0x0005;
        private const Int32 SIZE_RESTORED = 0;
        private const Int32 MONITOR_DEFAULTTONEAREST = 0x00000002;

        /// <summary>
        /// Listens out for all windows messages for this window
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                // Handle the GetMinMaxInfo of the Window
                case WM_GETMINMAXINFO:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;

                case WM_SIZE:
                {
                    int resizing = (int)wParam;

                    if (resizing == SIZE_RESTORED)
                    {
                        MonitorArea monitorArea = GetMonitorArea(hwnd);

                        if (monitorArea != null)
                        {
                            // LOWORD
                            int width = ((int)lParam & 0x0000ffff);

                            // HIWORD
                            int height = (int)((int)lParam & 0xffff0000) >> 16;

                            // Detect if window was snapped to screen side of current monitor
                            // or if spanning width on multiple monitors (to avoid unsnapping)
                            if (height == monitorArea.Work.Height ||
                                width >= SystemParameters.VirtualScreenWidth)
                            {
                                IsSnapped = true;
                            }
                            else
                            {
                                IsSnapped = false;
                            }
                        }
                    }
                }
                break;
            }

            return (IntPtr)0;
        }

        /// <summary>
        /// Get the current monitor area of the Window
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        private static MonitorArea GetMonitorArea(IntPtr hWnd)
        {
            var monitor = MonitorFromWindow(hWnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);

                return new MonitorArea(monitorInfo.rcMonitor, monitorInfo.rcWork);
            }

            return null;
        }

        #endregion Windows Proc

        /// <summary>
        /// Get the min/max window size for this window
        /// Correctly accounting for the taskbar size and position
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lParam"></param>
        private void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            // Get the point position to determine what screen we are on
            POINT lMousePosition;
            GetCursorPos(out lMousePosition);

            // Get the primary monitor at cursor position 0,0
            var lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);

            // Try and get the primary screen information
            var lPrimaryScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
                return;

            // Now get the current screen
            var lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);

            // If this has changed from the last one, update the transform
            if (lCurrentScreen != m_LastScreen || m_TransformToDevice == default(Matrix))
                GetTransform();

            // Store last know screen
            m_LastScreen = lCurrentScreen;

            // Get min/max structure to fill with information
            var lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // If it is the primary screen, use the rcWork variable
            if (lPrimaryScreen.Equals(lCurrentScreen) == true)
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
            }
            // Otherwise it's the rcMonitor values
            else
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right - lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom - lPrimaryScreenInfo.rcMonitor.Top;
            }

            // Set min size
            var minSize = m_TransformToDevice.Transform(new Point(m_Window.MinWidth, m_Window.MinHeight));

            lMmi.ptMinTrackSize.X = (int)minSize.X;
            lMmi.ptMinTrackSize.Y = (int)minSize.Y;

            // Store new size
            m_ScreenSize = new Rect(lMmi.ptMaxPosition.X, lMmi.ptMaxPosition.Y, lMmi.ptMaxSize.X, lMmi.ptMaxSize.Y);

            // Now we have the max size, allow the host to tweak as needed
            Marshal.StructureToPtr(lMmi, lParam, true);
        }
    }

    #region Dll Helper Structures

    internal enum MonitorOptions : uint
    {
        MONITOR_DEFAULTTONULL = 0x00000000,
        MONITOR_DEFAULTTOPRIMARY = 0x00000001,
        MONITOR_DEFAULTTONEAREST = 0x00000002
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        public RECT rcMonitor = new RECT();
        public RECT rcWork = new RECT();
        public int dwFlags = 0;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public int Left, Top, Right, Bottom;

        public Rectangle(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        /// <summary>
        /// x coordinate of point.
        /// </summary>
        public int X;

        /// <summary>
        /// y coordinate of point.
        /// </summary>
        public int Y;

        /// <summary>
        /// Construct a point of coordinates (x,y).
        /// </summary>
        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public class MonitorArea
    {
        public struct Region
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
            public int Width;
            public int Height;
        }

        public Region Work;
        public Region Display;

        public POINT Offset;

        public MonitorArea(RECT display, RECT work)
        {
            Display.Left = display.Left;
            Display.Right = display.Right;
            Display.Top = display.Top;
            Display.Bottom = display.Bottom;
            Display.Width = Math.Abs(display.Right - display.Left);
            Display.Height = Math.Abs(display.Bottom - display.Top);

            Work.Left = work.Left;
            Work.Right = work.Right;
            Work.Top = work.Top;
            Work.Bottom = work.Bottom;
            Work.Width = Math.Abs(work.Right - work.Left);
            Work.Height = Math.Abs(work.Bottom - work.Top);

            Offset = new POINT(Math.Abs(work.Left - display.Left),
                Math.Abs(work.Top - display.Top));
        }
    }

    #endregion Dll Helper Structures
}