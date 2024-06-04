using Modules.Common.Views.ValueConverters;
using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2;

/// <summary>
/// A converter that takes in 4 numbers at max and converts it into CornerRadius
/// </summary>
public class CornerRadiusConverter : BaseMultiValueConverter<CornerRadiusConverter>
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        double topLeft = values.Length > 0 ? System.Convert.ToDouble(values[0]) : 0;
        double topRight = values.Length > 1 ? System.Convert.ToDouble(values[1]) : 0;
        double bottomRight = values.Length > 2 ? System.Convert.ToDouble(values[2]) : 0;
        double bottomLeft = values.Length > 3 ? System.Convert.ToDouble(values[3]) : 0;

        return new CornerRadius(topLeft, topRight, bottomRight, bottomLeft);
    }
}
