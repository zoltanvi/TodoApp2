using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2
{
    internal class ColorBarGridWidthMultiValueConverter : BaseMultiValueConverter<ColorBarGridWidthMultiValueConverter>
    {
        private ColorBarBorderWidthMultiValueConverter m_MultiConverter = new ColorBarBorderWidthMultiValueConverter();

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)m_MultiConverter.Convert(values, targetType, parameter, culture);
            return new GridLength(width);
        }
    }
}
