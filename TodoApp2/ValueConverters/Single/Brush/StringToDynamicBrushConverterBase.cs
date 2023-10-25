using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{
    public class DynamicSolidColorBrushConverter : BaseValueConverter
    {
        private readonly StringRGBToBrushConverter _converter = new StringRGBToBrushConverter();
        private SolidColorBrush _lightBrush;
        private SolidColorBrush _darkBrush;

        public string LightName { get; set; }
        public string DarkName { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string colorString = (string)value;
            SolidColorBrush colorBrush = (SolidColorBrush)_converter.Convert(colorString, targetType, parameter, culture);

            if (colorBrush != null)
            {
                GetBrushes(out _lightBrush, out _darkBrush);
                var brightness = GetBrightness(colorBrush.Color);
                return brightness < 0.5 ? _lightBrush : _darkBrush;
            }

            return null;
        }


        /// <summary>
        /// Finds the light and dark brushes from the application resources and returns them.
        /// If the found dark brush is lighter than the found light brush, the brushes are swapped.
        /// </summary>
        /// <param name="lightBrush">The light brush</param>
        /// <param name="darkBrush">The dark brush</param>
        private void GetBrushes(out SolidColorBrush lightBrush, out SolidColorBrush darkBrush)
        {
            darkBrush = (SolidColorBrush)Application.Current.TryFindResource(DarkName);
            lightBrush = (SolidColorBrush)Application.Current.TryFindResource(LightName);

            var darkBrushDarkness = GetIntBrightness(darkBrush.Color);
            var lightBrushDarkness = GetIntBrightness(lightBrush.Color);

            if (darkBrushDarkness > lightBrushDarkness)
            {
                SolidColorBrush tempBrush = darkBrush;
                darkBrush = lightBrush;
                lightBrush = tempBrush;
            }
        }

        /// <summary>
        /// Gets the normalized brightness value of the color.
        /// </summary>
        /// <param name="color">The color to get its brightness</param>
        /// <returns>The brightness value (0.0 - 1.0)</returns>
        private float GetBrightness(Color color)
        {
            return GetIntBrightness(color) / 255.0f;
        }

        /// <summary>
        /// Gets the brightness of the color.
        /// </summary>
        /// <param name="color">The color to get its brightness</param>
        /// <returns>The brightness value (0-255)</returns>
        private int GetIntBrightness(Color color)
        {
            return (int)Math.Sqrt(
                color.R * color.R * 0.241 +
                color.G * color.G * 0.691 +
                color.B * color.B * 0.068);
        }
    }
}
