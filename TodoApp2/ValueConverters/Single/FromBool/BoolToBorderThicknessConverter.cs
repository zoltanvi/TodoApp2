using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2;

public class BoolToBorderThicknessConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (bool)value;
        return new Thickness(val ? 0 : 9);
    }
}
