using PropertyChanged;
using System;
using System.Windows;
using TodoApp2.Core;
using TodoApp2.Core.Extensions;
using TodoApp2.WindowHandling.Resizing;

namespace TodoApp2.Services.Window;

public class WindowService : BaseViewModel, IWindowService
{
    private readonly MainWindow _window;
    private readonly WindowResizer _resizer;
    private bool _closing;
    private WindowDockPosition _dockPosition = WindowDockPosition.Undocked;

    public WindowService(MainWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        _window = window;

        _window.Deactivated += OnWindowDeactivated;
        _window.StateChanged += OnWindowStateChanged;
        _window.Loaded += (s, e) => Loaded?.Invoke(this, EventArgs.Empty);
        _window.Closing += (s, e) =>
        {
            Closing?.Invoke(this, EventArgs.Empty);
            _closing = true;
        };
        _window.Closed += (s, e) => Closed?.Invoke(this, EventArgs.Empty);

        // Fix window resize issue
        _resizer = new WindowResizer(_window);
        _resizer.WindowDockChanged += OnWindowDockChanged;
        _resizer.IsDockedChanged += OnIsDockedChanged;
    }

    public bool Topmost
    {
        get => _window.Topmost;
        set
        {
            _window.Topmost = value;
            if (value)
            {
                _window.Show();
            }
        }
    }

    public bool IsDocked { get; private set; }
    public bool IsMinimized { get; private set; }
    public bool IsMaximized { get; private set; }
    public bool IsMaximizedOrDocked { get; private set; }
    public bool IsRoundedCornersAllowed { get; private set; }

    public double Left
    {
        get
        {
            var left = _window.Left;
            // If the window is maximized, window.Left gives back incorrect value
            if (_window.WindowState == System.Windows.WindowState.Maximized)
            {
                left = _window.GetFieldValue<double>("_actualLeft");
            }

            return left;

        }
        set => _window.Left = value;
    }

    public double Top
    {
        get
        {
            var top = _window.Top;
            // If the window is maximized, window.Top gives back incorrect value
            if (_window.WindowState == System.Windows.WindowState.Maximized)
            {
                top = _window.GetFieldValue<double>("_actualLeft");
            }

            return top;

        }
        set => _window.Top = value;
    }

    public double Width
    {
        get => _window.Width;
        set => _window.Width = value;
    }

    public double Height
    {
        get => _window.Height;
        set => _window.Height = value;
    }

    public WindowState WindowState
    {
        get => _window.WindowState;
        set => _window.WindowState = value;
    }

    public event EventHandler Deactivated;
    public event EventHandler StateChanged;
    public event EventHandler Loaded;
    public event EventHandler Closing;
    public event EventHandler Closed;
    public event EventHandler Resized;
    public event EventHandler RoundedCornersChanged;

    public void Activate() => _window.Activate();

    public void FlashWindow(bool topmost)
    {
        if (!_window.IsVisible)
        {
            Show();
        }

        if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }

        Activate();
        Topmost = true;  // important
        Topmost = false; // important
        Focus();         // important
        Topmost = topmost;

        _window.FlashWindow(3);
    }

    public void Close() => _window.Close();

    public void Focus() => _window.Focus();

    public void Show() => _window.Show();

    public void Hide() => _window.Hide();


    public void SetPosition(double left, double top)
    {
        _window.Left = left;
        _window.Top = top;
        UpdateRoundedCornersAllowed();
    }

    public void SetSize(double width, double height)
    {
        _window.Width = width;
        _window.Height = height;
        UpdateRoundedCornersAllowed();
    }

    public void Minimize() => _window.WindowState = System.Windows.WindowState.Minimized;
    public void Maximize() => _window.WindowState ^= System.Windows.WindowState.Maximized;

    /// <summary>
    /// If the window resizes to a special position (docked or maximized)
    /// this will update all required property change events to set the borders and radius values
    /// </summary>
    private void WindowResized()
    {
        IsMinimized = _window.WindowState == System.Windows.WindowState.Minimized;
        IsMaximized = _window.WindowState == System.Windows.WindowState.Maximized;
        IsMaximizedOrDocked = IsMaximized || IsDocked;

        UpdateRoundedCornersAllowed();
        Resized?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateRoundedCornersAllowed()
    {
        bool isDocked = IsDocked || _dockPosition != WindowDockPosition.Undocked;
        IsRoundedCornersAllowed = !(isDocked || IsMaximized);
        RoundedCornersChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnWindowDeactivated(object sender, EventArgs e)
    {
        if (!_closing)
        {
            Deactivated?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnWindowStateChanged(object sender, EventArgs e)
    {
        WindowResized();
    }

    private void OnWindowDockChanged(WindowDockPosition dockPosition)
    {
        // Store last position
        _dockPosition = dockPosition;

        // Fire off resize events
        WindowResized();
    }

    private void OnIsDockedChanged(object sender, DockChangeEventArgs e)
    {
        IsDocked = e.IsDocked;
        IsMaximizedOrDocked = IsMaximized || IsDocked;
        UpdateRoundedCornersAllowed();
        WindowResized();
    }
}
