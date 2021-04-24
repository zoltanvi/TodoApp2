using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a boolean and converts it to a WPF brush
    /// It is used for task list item background.
    /// </summary>
    public class BackgroundBrushConverter : BaseMultiValueConverter<BackgroundBrushConverter>
    {
        private static readonly SolidColorBrush s_Transparent = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isDone = (bool)values[0];
            bool isItemBackgroundVisible = (bool)values[1];
            
            // The task list item background is hatched when the task is done and the background is enabled in the settings
            // Brush is always got from resources because this way it can dynamically change during runtime
            if (!isItemBackgroundVisible) return s_Transparent;
            
            return isDone ? Application.Current.TryFindResource("HatchBrush") : Application.Current.TryFindResource("TaskItemBackgroundBrush");
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}