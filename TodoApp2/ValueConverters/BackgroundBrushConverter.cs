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
    public class BackgroundBrushConverter : BaseValueConverter<BackgroundBrushConverter>
    {
        private readonly Brush m_NormalBrush;
        private readonly Brush m_HatchedBrush;

        public BackgroundBrushConverter()
        {
            m_NormalBrush = (Brush)Application.Current.TryFindResource("TaskItemBackgroundBrush");
            m_HatchedBrush = (Brush)Application.Current.TryFindResource("HatchBrush");
            if (m_NormalBrush == null || m_HatchedBrush == null)
            {
                throw new NullReferenceException("Can't find TaskListItem background brush!");
            }
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isDone = (bool)value;

            // Converts the boolean into a brush
            // The task list item background is hatched when the task is done
            return isDone ? m_HatchedBrush : m_NormalBrush;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}