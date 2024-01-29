using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TodoApp2.Core;
using Window = System.Windows.Window;

namespace TodoApp2
{
    public class GridResizerVertical : BaseViewModel
    {
        private const double UnscaledSnappingHeight = 42;
        private readonly double _unscaledMinRowHeight = 42;

        private Guid _doubleClickTimer;
        private readonly Grid _grid;
        private readonly GridSplitter _resizer;
        private readonly Window _window;
        private bool _isDragging;
        private bool _isDraggingEnabled = true;

        public bool IsGridInitialized => _grid.ActualHeight != 0;
        public double MinRowHeight => _unscaledMinRowHeight * UIScaler.Instance.ScaleValue;
        public double GridHalfHeight => _grid.ActualHeight / 2;
        public double MaxRowHeight => GridHalfHeight < MinRowHeight ? MinRowHeight : GridHalfHeight;
        public double SnappingHeight => UnscaledSnappingHeight * UIScaler.Instance.ScaleValue;

        private GridResizerDebugWindow _gridResizerDebugWindow;

        public double FirstRowHeight => _grid.RowDefinitions[0].ActualHeight;
        public double SecondRowHeight => _grid.RowDefinitions[1].ActualHeight;
        public double ThirdRowHeight => _grid.RowDefinitions[2].ActualHeight;

        public double BottomRowHeight
        {
            get => _grid.RowDefinitions[_grid.RowDefinitions.Count - 1].ActualHeight;
            set
            {
                double clampedValue = value;

                // Snap below snapping height
                if (clampedValue <= SnappingHeight)
                {
                    clampedValue = 0;
                }
                // Do nothing between snapping height and minimum height
                else if (clampedValue > SnappingHeight && clampedValue < MinRowHeight)
                {
                    clampedValue = MinRowHeight;
                }
                // Stop at maximum height (at 1/2)
                else if (IsGridInitialized && clampedValue > MaxRowHeight)
                {
                    clampedValue = MaxRowHeight;
                }

                _grid.RowDefinitions[_grid.RowDefinitions.Count - 1].Height = new GridLength(clampedValue);

                OnPropertyChanged(nameof(FirstRowHeight));
                OnPropertyChanged(nameof(SecondRowHeight));
                OnPropertyChanged(nameof(ThirdRowHeight));

                OnPropertyChanged(nameof(IsGridInitialized));
                OnPropertyChanged(nameof(MinRowHeight));
                OnPropertyChanged(nameof(GridHalfHeight));
                OnPropertyChanged(nameof(MaxRowHeight));
                OnPropertyChanged(nameof(SnappingHeight));
            }
        }

        public bool IsPanelOpen { get; set; }

        private double _lastSavedRowHeight;

        public double LastSavedRowHeight
        {
            get => _lastSavedRowHeight;
            set
            {
                if (value != 0)
                {
                    _lastSavedRowHeight = value;
                    IsPanelOpen = true;
                }
                else
                {
                    IsPanelOpen = false;
                }
            }
        }

        public GridResizerVertical(Grid grid, GridSplitter resizer, Window window)
        {
            _grid = grid;
            _resizer = resizer;
            _window = window;

            _doubleClickTimer = TimerService.Instance.CreateTimer(100, OnDoubleClickTimer);
            BottomRowHeight = 42;

            _resizer.DragDelta += GridSplitter_DragDelta;
            _resizer.DragStarted += GridSplitter_DragStarted;
            _resizer.DragCompleted += GridSplitter_DragCompleted;
            _resizer.MouseDoubleClick += GridSplitter_MouseDoubleClick;
            _grid.MouseMove += Grid_MouseMove;

            _window.SizeChanged += WindowSizeChanged;
            _window.StateChanged += Window_StateChanged;

            UIScaler.Instance.Zoomed += Instance_Zoomed;

            _gridResizerDebugWindow = new GridResizerDebugWindow();
            _gridResizerDebugWindow.DataContext = this;
            _gridResizerDebugWindow.Show();
        }

        private void OnDoubleClickTimer(object sender, EventArgs e)
        {
            TimerService.Instance.StopTimer(_doubleClickTimer);

            // Since the dragging and double click handling gets tangled,
            // there is a slight delay for enabling dragging after a double click event
            _isDraggingEnabled = true;
        }

        private async void Window_StateChanged(object sender, EventArgs e)
        {
            await Task.Delay(10);

            // if side menu is too wide
            if (BottomRowHeight > GridHalfHeight)
            {
                // default: close
                double newHeight = 0;

                // if closing is not needed
                if (GridHalfHeight >= MinRowHeight)
                {
                    // set new max limit as height
                    newHeight = MaxRowHeight;
                }

                BottomRowHeight = newHeight;
                LastSavedRowHeight = newHeight;
            }
        }

        private void Instance_Zoomed(object sender, ZoomedEventArgs e)
        {
            double originalHeight = BottomRowHeight / e.OldScaleValue;
            double newHeight = originalHeight * e.NewScaleValue;
            BottomRowHeight = newHeight;
        }

        private void GridSplitter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _isDraggingEnabled = false;

            if (IsPanelOpen)
            {
                BottomRowHeight = 0;
            }
            else
            {
                BottomRowHeight = LastSavedRowHeight;
            }

            IsPanelOpen = !IsPanelOpen;

            TimerService.Instance.StartTimer(_doubleClickTimer);
        }

        private void RecalculatePanelHeight()
        {
            if (LastSavedRowHeight > MaxRowHeight)
            {
                LastSavedRowHeight = MaxRowHeight;
            }

            if (LastSavedRowHeight < MinRowHeight)
            {
                LastSavedRowHeight = MinRowHeight;
            }

            BottomRowHeight = LastSavedRowHeight;
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            bool heightShrank = e.HeightChanged && e.PreviousSize.Height > e.NewSize.Height;
            if (heightShrank)
            {
                // if side menu is too wide
                if (BottomRowHeight > GridHalfHeight)
                {
                    // default: close
                    double newHeight = 0;

                    // if closing is not needed
                    if (GridHalfHeight >= MinRowHeight)
                    {
                        // set new max limit as height
                        newHeight = MaxRowHeight;
                    }

                    BottomRowHeight = newHeight;
                    LastSavedRowHeight = newHeight;
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
                double newHeight = BottomRowHeight - e.VerticalChange;
                BottomRowHeight = newHeight;
                _isDragging = true;
            }
        }

        private void GridSplitter_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;

            if (_isDraggingEnabled)
            {
                // Persist control height into app settings
                LastSavedRowHeight = BottomRowHeight;
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _isDraggingEnabled)
            {
                double newHeight = _grid.ActualHeight - e.GetPosition(_grid).Y;
                BottomRowHeight = newHeight;
            }
        }
    }
}
