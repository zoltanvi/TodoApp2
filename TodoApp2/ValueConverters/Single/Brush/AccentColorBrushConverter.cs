using System;
using System.Globalization;
using System.Windows.Media;
using TodoApp2.Core;

namespace TodoApp2
{
    public class AccentColorBrushConverter : BaseValueConverter
    {
        private readonly StringRGBToBrushConverter _converter = new StringRGBToBrushConverter();

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorCode)
            {
                if (colorCode == GlobalConstants.ColorName.Transparent)
                {
                    return _converter.Convert(GlobalConstants.ColorName.DefaultAccentColor, targetType, parameter, culture);
                }

                return _converter.Convert(value, targetType, parameter, culture);
            }

            return null;
        }

        public SolidColorBrush Convert(string value)
        {
            return (SolidColorBrush)Convert(value, typeof(SolidColorBrush), null, null);
        }
    }
}