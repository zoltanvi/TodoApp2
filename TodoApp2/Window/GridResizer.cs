using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TodoApp2.Core;

namespace TodoApp2
{
    public class GridResizer
    {
        private const double SnappingWidth = 60;
        private Grid _Grid;
        private GridSplitter _Resizer;
        private Window _Window;
        private bool _IsDragging;
        
        private AppViewModel AppViewModel => IoC.ApplicationViewModel;
        private double MaxColumnWidth => (_Grid.ActualWidth / 3) * 2;
        private double MinColumnWidth => IoC.UIScaler.SideMenuMinimumWidth;
        
        private double LeftColumnWidth
        {
            get => _Grid.ColumnDefinitions[0].ActualWidth;
            set => _Grid.ColumnDefinitions[0].Width = new GridLength(value);
        }
        
        private double AppSettingsSideMenuWidth
        {
            get => AppViewModel.ApplicationSettings.SideMenuWidth;
            set => AppViewModel.ApplicationSettings.SideMenuWidth = value;
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

            LeftColumnWidth = AppSettingsSideMenuWidth;

            _Window.SizeChanged += WindowSizeChanged;

            Mediator.Register(OnSideMenuButtonClicked, ViewModelMessages.SideMenuButtonClicked);
        }

        private void OnSideMenuButtonClicked(object obj)
        {
            if (LeftColumnWidth == 0)
            {
                LeftColumnWidth = MinColumnWidth;
            }
            else
            {
                LeftColumnWidth = 0;
            }
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            bool widthShrank = e.WidthChanged && e.PreviousSize.Width > e.NewSize.Width;
            if (widthShrank)
            {
                // if side menu is too wide
                if (LeftColumnWidth > MaxColumnWidth)
                {
                    // default: close
                    double newWidth = 0;

                    // if closing is not needed
                    if (MaxColumnWidth >= MinColumnWidth)
                    {
                        // set new max limit as width
                        newWidth = MaxColumnWidth;
                    }

                    _Grid.ColumnDefinitions[0].Width = new GridLength(newWidth);
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

            _Grid.ColumnDefinitions[0].Width = CalculateNewWidth(newWidth);
            _IsDragging = true;
        }

        private void GridSplitter_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _IsDragging = false;

            // Persist control width into app settings
            AppSettingsSideMenuWidth = _Grid.ColumnDefinitions[0].Width.Value;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_IsDragging)
            {
                double newWidth = e.GetPosition(_Grid).X;

                _Grid.ColumnDefinitions[0].Width = CalculateNewWidth(newWidth);
            }
        }

        private GridLength CalculateNewWidth(double width)
        {
            // Snap below snapping width
            if (width < SnappingWidth)
            {
                width = 0;
            }
            // Do nothing between snapping width and minimum side menu widh
            else if (width > SnappingWidth && width < MinColumnWidth)
            {
                width = MinColumnWidth;
            }
            // Stop at maximum width (at 2/3)
            else if (width > MaxColumnWidth)
            {
                width = MaxColumnWidth;
            }

            return new GridLength(width);
        }
    }
}
