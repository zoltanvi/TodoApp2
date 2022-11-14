using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{
    public class AccentColorBrushConverter : BaseValueConverter
    {
        private const string s_DefaultAccentColor = "#0291cd";
        private readonly StringRGBToBrushConverter m_Converter = new StringRGBToBrushConverter();

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string brushName)
            {
                if (brushName == "Transparent")
                {
                    return m_Converter.Convert(s_DefaultAccentColor, targetType, parameter, culture);
                }

                return m_Converter.Convert(value, targetType, parameter, culture);
            }

            return null;
        }
    }
}