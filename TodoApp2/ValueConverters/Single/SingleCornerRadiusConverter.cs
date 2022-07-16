using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a single number and converts it into CornerRadius
    /// </summary>
    public class SingleCornerRadiusConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new CornerRadius(System.Convert.ToDouble(value));
        }
    }
}
