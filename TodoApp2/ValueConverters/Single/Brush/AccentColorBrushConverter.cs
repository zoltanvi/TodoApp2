using System;
using System.Globalization;
using TodoApp2.Core.Constants;

namespace TodoApp2
{
    public class AccentColorBrushConverter : BaseValueConverter
    {
        private readonly StringRGBToBrushConverter m_Converter = new StringRGBToBrushConverter();

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string brushName)
            {
                if (brushName == GlobalConstants.ColorName.Transparent)
                {
                    return m_Converter.Convert(GlobalConstants.ColorName.DefaultAccentColor, targetType, parameter, culture);
                }

                return m_Converter.Convert(value, targetType, parameter, culture);
            }

            return null;
        }
    }
}