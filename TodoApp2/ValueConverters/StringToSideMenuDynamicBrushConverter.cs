using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a string and converts it to a WPF brush
    /// It is used for the controls in the side menu.
    /// </summary>
    public class StringToSideMenuDynamicBrushConverter : BaseValueConverter<StringToSideMenuDynamicBrushConverter>
    {
        private readonly StringRGBToBrushConverter m_Converter = new StringRGBToBrushConverter();

        public override object Convert(object values, Type targetType, object parameter, CultureInfo culture)
        {
            string colorString = (string)values;
            var lightBrush = Application.Current.TryFindResource("SideMenuForegroundBrush");
            var darkBrush = Application.Current.TryFindResource("SideMenuBackgroundBrush");

            SolidColorBrush colorBrush = (SolidColorBrush)m_Converter.Convert(colorString, targetType, parameter, culture);
            
            if (colorBrush != null)
            {
                var brightness = GetBrightness(colorBrush.Color);
                return brightness < 0.4 ? lightBrush : darkBrush;
            }

            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private float GetBrightness(Color color)
        {
            return GetIntBrightness(color) / 255.0f;
        }

        private int GetIntBrightness(Color color)
        {
            return (int)Math.Sqrt(
                color.R * color.R * 0.241 +
                color.G * color.G * 0.691 +
                color.B * color.B * 0.068);
        }

    }
}
