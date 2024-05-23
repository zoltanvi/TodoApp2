using Modules.Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using TodoApp2.Core;

namespace TodoApp2;

/// <summary>
/// A converter that takes in 2 booleans and returns a WPF brush.
/// If the second bool is false, a transparent brush is returned.
/// </summary>
public class BackgroundBrushConverter : BaseMultiValueConverter<BackgroundBrushConverter>
{
    private static readonly SolidColorBrush Transparent = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

    private VisualBrush _hatchBrush;
    private Path _path;

    public BackgroundBrushConverter()
    {
        _path = new Path
        {
            Data = Geometry.Parse("M 0 8 L 8 0"),
            Stroke = (Brush)Application.Current.TryFindResource(Constants.BrushName.Surface3),
            StrokeEndLineCap = PenLineCap.Square,
        };

        _hatchBrush = new VisualBrush
        {
            TileMode = TileMode.Tile,
            Viewbox = new Rect(0, 0, 8, 8),
            ViewboxUnits = BrushMappingMode.Absolute,
            Viewport = new Rect(0, 0, 8, 8),
            ViewportUnits = BrushMappingMode.Absolute,
            Visual = _path
        };
        //RenderOptions.SetEdgeMode(hatchBrush.Visual, EdgeMode.Aliased);
    }

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is bool isDone && values[1] is bool isBackgroundVisible)
        {
            // The task list item background is hatched when the task is done and the background is enabled in the settings
            // Brush is always got from resources because this way it can dynamically change during runtime
            if (!isBackgroundVisible) return Transparent;

            if (isDone)
            {
                _path.Stroke = (Brush)Application.Current.TryFindResource(Constants.BrushName.Surface3);
                return _hatchBrush;
            }
            else
            {
                return Application.Current.TryFindResource(Constants.BrushName.TaskBgBrush);
            }
        }

        return new SolidColorBrush(Colors.Magenta);
    }
}