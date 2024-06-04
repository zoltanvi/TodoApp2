using Modules.Common.Views.ValueConverters;
using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2;

internal class BoolToVisibilityAndConverter : BaseMultiValueConverter<BoolToVisibilityAndConverter>
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
            return Visibility.Collapsed;

            // TODO: Check binding errors
            //throw new ArgumentException($"FirstValue: {values[0]?.GetType()}, SecondValue: {values[1]?.GetType()}.");
        }
    }
}

internal class BoolToVisibilityOrConverter : BaseMultiValueConverter<BoolToVisibilityOrConverter>
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2)
        {
            return Visibility.Collapsed;
        }

        if (values[0] is bool firstBool && values[1] is bool secondBool)
        {
            return (firstBool) || (secondBool) ? Visibility.Visible : Visibility.Collapsed;
        }
        else
        {
            return Visibility.Collapsed;

            // TODO: Check binding errors
            //throw new ArgumentException($"FirstValue: {values[0]?.GetType()}, SecondValue: {values[1]?.GetType()}.");
        }

    }
}
