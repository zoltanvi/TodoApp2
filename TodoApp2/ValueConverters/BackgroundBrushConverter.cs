using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a boolean and converts it to a WPF brush
    /// It is used for task list item background.
    /// </summary>
    public class BackgroundBrushConverter : BaseValueConverter<BackgroundBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isDone = (bool)value;

            // Converts the boolean into a brush
            // The task list item background is hatched when the task is done
            // Brush is always got from resources because this way it can dynamically change during runtime
            return isDone ? CreateHatchBrush() : (Brush)Application.Current.TryFindResource("TaskItemBackgroundBrush");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private Brush CreateHatchBrush()
        {
            var path = new Path()
            {
                Data = Geometry.Parse("M 0 8 L 8 0"),
                Stroke = (Brush)Application.Current.TryFindResource("TaskItemHatchedHatchBrush"),
                StrokeEndLineCap = PenLineCap.Square,
            };

            RenderOptions.SetEdgeMode(path, EdgeMode.Aliased);

            return new VisualBrush()
            {
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, 8, 8),
                ViewportUnits = BrushMappingMode.Absolute,
                Viewbox = new Rect(0, 0, 8, 8),
                ViewboxUnits = BrushMappingMode.Absolute,
                Visual = path
            };
        }
    }
}