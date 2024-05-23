using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2;

public class DoubleToGridLengthConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return new GridLength((double)value);
    }
}
