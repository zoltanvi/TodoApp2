using System;
using System.Globalization;
using System.Windows;

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
            return isDone ? Application.Current.TryFindResource("HatchBrush") : Application.Current.TryFindResource("TaskItemBackgroundBrush");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}