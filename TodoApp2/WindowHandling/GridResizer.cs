using Modules.Settings.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TodoApp2.Core;
using Window = System.Windows.Window;

namespace TodoApp2.WindowHandling;

public class GridResizer
{
    private const double UnscaledSnappingWidth = 60;
    private readonly double _unscaledMinColumnWidth = 180;

    private Guid _doubleClickTimer;
    private readonly Grid _grid;
    private readonly GridSplitter _resizer;
    private readonly Window _window;
    private bool _isDragging;
    private bool _isDraggingEnabled = true;

    private bool IsGridInitialized => _grid.ActualWidth != 0;
    private double MinColumnWidth => _unscaledMinColumnWidth * UIScaler.Instance.ScaleValue;
    private double GridHalfWidth => _grid.ActualWidth / 2;
    private double MaxColumnWidth => GridHalfWidth < MinColumnWidth ? MinColumnWidth : GridHalfWidth;
    private double SnappingWidth => UnscaledSnappingWidth * UIScaler.Instance.ScaleValue;
    private SessionSettings SessionSettings => IoC.AppSettings.SessionSettings;

    private double LeftColumnWidth
    {
        get => _grid.ColumnDefinitions[0].ActualWidth;
        set
        {
            var clampedValue = value;

            // Snap below snapping width
            if (clampedValue <= SnappingWidth)
            {
                clampedValue = 0;
            }
            // Do nothing between snapping width and minimum side menu widh
            else if (clampedValue > SnappingWidth && clampedValue < MinColumnWidth)
            {
                clampedValue = MinColumnWidth;
            }
            // Stop at maximum width (at 1/2)
            else if (IsGridInitialized && clampedValue > MaxColumnWidth)
            {
                clampedValue = MaxColumnWidth;
            }

            _grid.ColumnDefinitions[0].Width = new GridLength(clampedValue);
        }
    }

    private bool IsSideMenuOpen
    {
        get => SessionSettings.SideMenuOpen;
        set => SessionSettings.SideMenuOpen = value;
    }

    private double LastSavedColumnWidth
    {
        get => SessionSettings.SideMenuWidth;
        set
        {
            if (value != 0)
            {
                SessionSettings.SideMenuWidth = value;
                IsSideMenuOpen = true;
            }
            else
            {
                IsSideMenuOpen = false;
            }
        }
    }

    public GridResizer(Grid grid, GridSplitter resizer, Window window)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(resizer);
        ArgumentNullException.ThrowIfNull(window);

        _grid = grid;
        _resizer = resizer;
        _window = window;

        _doubleClickTimer = TimerService.Instance.CreateTimer(100, OnDoubleClickTimer);
        InitializeLeftColumnWidth();

        _resizer.DragDelta += GridSplitter_DragDelta;
        _resizer.DragStarted += GridSplitter_DragStarted;
        _resizer.DragCompleted += GridSplitter_DragCompleted;
        _resizer.MouseDoubleClick += GridSplitter_MouseDoubleClick;
        _grid.MouseMove += Grid_MouseMove;

        _window.SizeChanged += WindowSizeChanged;
        _window.StateChanged += Window_StateChanged;

        UIScaler.Instance.Zoomed += Instance_Zoomed;
        Mediator.Register(OnSideMenuButtonClicked, ViewModelMessages.SideMenuButtonClicked);
        Mediator.Register(OnSideMenuCloseRequested, ViewModelMessages.SideMenuCloseRequested);
    }

    private void OnDoubleClickTimer(object sender, EventArgs e)
    {
        TimerService.Instance.StopTimer(_doubleClickTimer);

        // Since the dragging and double click handling gets tangled,
        // there is a slight delay for enabling dragging after a double click event
        _isDraggingEnabled = true;
    }

    private void InitializeLeftColumnWidth()
    {
        if (SessionSettings.SideMenuOpen)
        {
            LeftColumnWidth = LastSavedColumnWidth;
        }
        else
        {
            LeftColumnWidth = 0;
        }
    }

    private async void Window_StateChanged(object sender, EventArgs e)
    {
        await Task.Delay(10);

        // if side menu is too wide
        if (LeftColumnWidth > GridHalfWidth)
        {
            // default: close
            double newWidth = 0;

            // if closing is not needed
            if (GridHalfWidth >= MinColumnWidth)
            {
                // set new max limit as width
                newWidth = MaxColumnWidth;
            }

            LeftColumnWidth = newWidth;
            LastSavedColumnWidth = newWidth;
        }
    }

    private void Instance_Zoomed(object sender, ZoomedEventArgs e)
    {
        var originalWidth = LeftColumnWidth / e.OldScaleValue;
        var newWidth = originalWidth * e.NewScaleValue;
        LeftColumnWidth = newWidth;
    }

    private void OnSideMenuButtonClicked(object obj)
    {
        if (LeftColumnWidth == 0)
        {
            RecalculateLeftColumnWidth();
            IsSideMenuOpen = true;
        }
        else
        {
            LeftColumnWidth = 0;
            IsSideMenuOpen = false;
        }
    }

    private void GridSplitter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        _isDraggingEnabled = false;

        if (IsSideMenuOpen)
        {
            LeftColumnWidth = 0;
        }
        else
        {
            LeftColumnWidth = LastSavedColumnWidth;
        }

        IsSideMenuOpen = !IsSideMenuOpen;

        TimerService.Instance.StartTimer(_doubleClickTimer);
    }

    private void OnSideMenuCloseRequested(object obj)
    {
        LeftColumnWidth = 0;
        IsSideMenuOpen = false;
    }

    private void RecalculateLeftColumnWidth()
    {
        if (LastSavedColumnWidth > MaxColumnWidth)
        {
            LastSavedColumnWidth = MaxColumnWidth;
        }

        if (LastSavedColumnWidth < MinColumnWidth)
        {
            LastSavedColumnWidth = MinColumnWidth;
        }

        LeftColumnWidth = LastSavedColumnWidth;
    }

    private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var widthShrank = e.WidthChanged && e.PreviousSize.Width > e.NewSize.Width;
        if (widthShrank)
        {
            // if side menu is too wide
            if (LeftColumnWidth > GridHalfWidth)
            {
                // default: close
                double newWidth = 0;

                // if closing is not needed
                if (GridHalfWidth >= MinColumnWidth)
                {
                    // set new max limit as width
                    newWidth = MaxColumnWidth;
                }

                LeftColumnWidth = newWidth;
                LastSavedColumnWidth = newWidth;
            }
        }
    }

    private void GridSplitter_DragStarted(object sender, DragStartedEventArgs e)
    {
        _isDragging = _isDraggingEnabled;
    }

    private void GridSplitter_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (_isDraggingEnabled)
        {
            var newWidth = LeftColumnWidth + e.HorizontalChange;
            LeftColumnWidth = newWidth;
            _isDragging = true;
        }
    }

    private void GridSplitter_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        _isDragging = false;

        if (_isDraggingEnabled)
        {
            // Persist control width into app settings
            LastSavedColumnWidth = LeftColumnWidth;
        }
    }

    private void Grid_MouseMove(object sender, MouseEventArgs e)
    {
        if (_isDragging && _isDraggingEnabled)
        {
            var newWidth = e.GetPosition(_grid).X;
            LeftColumnWidth = newWidth;
        }
    }
}
