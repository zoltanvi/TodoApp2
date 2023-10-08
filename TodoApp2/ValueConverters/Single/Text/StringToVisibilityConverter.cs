using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2
{
    public class StringToVisibilityConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }
    }
}
