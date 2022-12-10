using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using TodoApp2.Core.Constants;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in 2 booleans and returns a WPF brush.
    /// If the second bool is false, a transparent brush is returned.
    /// </summary>
    public class BackgroundBrushConverter : BaseMultiValueConverter<BackgroundBrushConverter>
    {
        private static readonly SolidColorBrush s_Transparent = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is bool isDone && values[1] is bool isBackgroundVisible)
            {
                // The task list item background is hatched when the task is done and the background is enabled in the settings
                // Brush is always got from resources because this way it can dynamically change during runtime
                if (!isBackgroundVisible) return s_Transparent;

                return isDone 
                    ? Application.Current.TryFindResource(GlobalConstants.BrushName.HatchBrush) 
                    : Application.Current.TryFindResource(GlobalConstants.BrushName.TaskBgBrush);
            }

            return false;
        }
    }
}