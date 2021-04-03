using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a long and converts it to a visibility enum
    /// </summary>
    public class LongToVisibilityConverter : BaseValueConverter<LongToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((long)value > 0)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}