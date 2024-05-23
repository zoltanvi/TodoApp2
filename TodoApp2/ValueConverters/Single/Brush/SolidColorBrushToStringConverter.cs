using System;
using System.Globalization;

namespace TodoApp2;

public class SolidColorBrushToStringConverter : BaseValueConverter
{
    private readonly StringRGBToBrushConverter _converter = new StringRGBToBrushConverter();

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return _converter.ConvertBack(value, targetType, parameter, culture);
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return _converter.Convert(value, targetType, parameter, culture);
    }
}
