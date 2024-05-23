using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2;

internal class ColorBarGridWidthMultiValueConverter : BaseMultiValueConverter<ColorBarGridWidthMultiValueConverter>
{
    private ColorBarBorderWidthMultiValueConverter _multiConverter = new ColorBarBorderWidthMultiValueConverter();

    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        double width = (double)_multiConverter.Convert(values, targetType, parameter, culture);
        return new GridLength(width);
    }
}
