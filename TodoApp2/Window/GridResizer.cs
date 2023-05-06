using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TodoApp2.Core;
using Window = System.Windows.Window;

namespace TodoApp2
{
    public class GridResizer
    {
        private const double UnscaledSnappingWidth = 60;
        
        private Grid _Grid;
        private GridSplitter _Resizer;
        private Window _Window;
        private bool _IsDragging;
        private double _UnscaledMinColumnWidth = 180;
        private double _UnscaledLeftColumnWidth = 0;
        
        private double _LeftColumnWidth => _Grid.ColumnDefinitions[0].Width.Value;
        private double MinColumnWidth => _UnscaledMinColumnWidth * UIScaler.Instance.ScaleValue;
        private double GridHalfWidth => _Grid.ActualWidth / 2;
        private double MaxColumnWidth => GridHalfWidth < MinColumnWidth ? MinColumnWidth : GridHalfWidth;
        private double SnappingWidth => UnscaledSnappingWidth * UIScaler.Instance.ScaleValue;
        private AppViewModel AppViewModel => IoC.ApplicationViewModel;

        private double LeftColumnWidth
        {
            get => _Grid.ColumnDefinitions[0].ActualWidth;
            set
            {
                double clampedValue = value;

                // Snap below snapping width
                if (clampedValue < SnappingWidth)
                {
                    clampedValue = 0;
                }
                // Do nothing between snapping width and minimum side menu widh
                else if (clampedValue > SnappingWidth && clampedValue < MinColumnWidth)
                {
                    clampedValue = MinColumnWidth;
                }
                // Stop at maximum width (at 1/2)
                else if (clampedValue > MaxColumnWidth)
                {
                    clampedValue = MaxColumnWidth;
                }

                _Grid.ColumnDefinitions[0].Width = new GridLength(clampedValue);
            }
        }

        private double LastSavedColumnWidth
        {
            get => AppViewModel.ApplicationSettings.SideMenuWidth;
            set
            {
                if (value != 0)
                {
                    AppViewModel.ApplicationSettings.SideMenuWidth = value;
                }
            }
        }

        public GridResizer(Grid grid, GridSplitter resizer, Window window)
        {
            _Grid = grid;
            _Resizer = resizer;
            _Window = window;

            _Resizer.DragDelta += GridSplitter_DragDelta;
            _Resizer.DragStarted += GridSplitter_DragStarted;
            _Resizer.DragCompleted += GridSplitter_DragCompleted;
            _Grid.MouseMove += Grid_MouseMove;

            LeftColumnWidth = LastSavedColumnWidth;
            _UnscaledLeftColumnWidth = _LeftColumnWidth;

            _Window.SizeChanged += WindowSizeChanged;
            _Window.StateChanged += Window_StateChanged;

            UIScaler.Instance.Zoomed += Instance_Zoomed; ;
            Mediator.Register(OnSideMenuButtonClicked, ViewModelMessages.SideMenuButtonClicked);
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
                _UnscaledLeftColumnWidth = _LeftColumnWidth;
                LastSavedColumnWidth = newWidth;
            }
        }

        private void Instance_Zoomed(object sender, ZoomedEventArgs e)
        {
            double originalWidth = LeftColumnWidth / e.OldScaleValue;
            double newWidth = originalWidth * e.NewScaleValue;
            LeftColumnWidth = newWidth;
        }

        private void OnSideMenuButtonClicked(object obj)
        {
            if (LeftColumnWidth == 0)
            {
                RecalculateLeftColumnWidth();
            }
            else
            {
                LeftColumnWidth = 0;
                _UnscaledLeftColumnWidth = _LeftColumnWidth;
            }
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
            _UnscaledLeftColumnWidth = _LeftColumnWidth;
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            bool widthShrank = e.WidthChanged && e.PreviousSize.Width > e.NewSize.Width;
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
                    _UnscaledLeftColumnWidth = _LeftColumnWidth;
                    LastSavedColumnWidth = newWidth;
                }
            }
        }

        private void GridSplitter_DragStarted(object sender, DragStartedEventArgs e)
        {
            _IsDragging = true;
        }

        private void GridSplitter_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newWidth = LeftColumnWidth + e.HorizontalChange;
            LeftColumnWidth = newWidth;
            _UnscaledLeftColumnWidth = _LeftColumnWidth;
            _IsDragging = true;
        }

        private void GridSplitter_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _IsDragging = false;

            // Persist control width into app settings
            LastSavedColumnWidth = LeftColumnWidth;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_IsDragging)
            {
                double newWidth = e.GetPosition(_Grid).X;
                LeftColumnWidth = newWidth;
                _UnscaledLeftColumnWidth = _LeftColumnWidth;
            }
        }
    }
}
