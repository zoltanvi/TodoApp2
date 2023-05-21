using System;
using System.Globalization;
using System.Windows.Media;

namespace TodoApp2
{
    public class DebugConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This converter is only used for debugging
            return value;
            //return (Color)ColorConverter.ConvertFromString((string)value);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;

            //if (value is Color color)
            //{
            //    return color.ToString();
            //}

            //return value;
        }
    }
}
