using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2
{
    internal class BoolToVisibilityMultiValueConverter : BaseMultiValueConverter<BoolToVisibilityMultiValueConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
            {
                return Visibility.Collapsed;
            }

            return ((bool)values[0]) && ((bool)values[1]) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
