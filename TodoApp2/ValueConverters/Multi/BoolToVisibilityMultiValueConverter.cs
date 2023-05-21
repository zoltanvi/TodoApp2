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

            if (values[0] is bool firstBool && values[1] is bool secondBool)
            {
                return (firstBool) && (secondBool) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;

                // TODO: Check binding errors
                //throw new ArgumentException($"FirstValue: {values[0]?.GetType()}, SecondValue: {values[1]?.GetType()}.");
            }
        }
    }

    internal class BoolToVisibilityMultiValueConverter2 : BaseMultiValueConverter<BoolToVisibilityMultiValueConverter2>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
            {
                return Visibility.Collapsed;
            }

            return ((bool)values[0]) || ((bool)values[1]) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
