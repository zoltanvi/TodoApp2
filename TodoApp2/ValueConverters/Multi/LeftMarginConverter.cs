using Modules.Common.Views.ValueConverters;
using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2;

internal class LeftMarginConverter : BaseMultiValueConverter<LeftMarginConverter>
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2)
        {
            return new Thickness();
        }
        double actualWidth = (double)values[0];
        Thickness margin = (Thickness)values[1];

        return new Thickness(actualWidth + margin.Left, 0, -(margin.Left + actualWidth), 0);
    }
}
