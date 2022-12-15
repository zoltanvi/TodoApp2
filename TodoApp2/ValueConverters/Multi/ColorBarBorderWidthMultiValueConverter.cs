using System;
using System.Globalization;
using TodoApp2.Core;

namespace TodoApp2
{
    internal class ColorBarBorderWidthMultiValueConverter : BaseMultiValueConverter<ColorBarBorderWidthMultiValueConverter>
    {
        private ThicknessToDoubleConverter m_ThicknessConverter = new ThicknessToDoubleConverter();

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is Thickness thickness && values[1] is double scaleValue)
            {
                double width = (double)m_ThicknessConverter.Convert(thickness, targetType, parameter, culture);
                return width * scaleValue;
            }

            return 0d;
        }
    }
}
