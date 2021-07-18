using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a width and a height value and converts them into a <see cref="Rect"/>
    /// </summary>
    public class WidthAndHeightToRectConverter : BaseMultiValueConverter<WidthAndHeightToRectConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)values[0];
            double height = (double)values[1];
            return new Rect(0, 0, width, height);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
