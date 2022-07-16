using System;
using System.Globalization;

namespace TodoApp2
{
    public class SolidColorBrushToStringConverter : BaseValueConverter
    {
        private readonly StringRGBToBrushConverter m_Converter = new StringRGBToBrushConverter();

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return m_Converter.ConvertBack(value, targetType, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return m_Converter.Convert(value, targetType, parameter, culture);
        }
    }
}
