using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using TodoApp2.Core;

namespace TodoApp2;

public class TransparentPatternBrushConverter : BaseValueConverter
{
    private readonly StringRGBToBrushConverter _converter = new StringRGBToBrushConverter();

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null ||
            value is string brushName &&
            (brushName == CoreConstants.ColorName.Transparent || string.IsNullOrEmpty(brushName)))
        {
            return (LinearGradientBrush)Application.Current.TryFindResource(CoreConstants.BrushName.TransparentPatternBrush);
        }
        else if (value is Brush)
        {
            return value;
        }

        return _converter.Convert(value, targetType, parameter, culture);
    }
}
