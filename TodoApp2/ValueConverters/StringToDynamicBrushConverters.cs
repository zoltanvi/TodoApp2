using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{
    public class DynamicBrushConverterTitleBar : BaseValueConverter<DynamicBrushConverterTitleBar>
    {
        private readonly StringToDynamicBrushConverterBase m_Converter;

        public DynamicBrushConverterTitleBar()
        {
            m_Converter = new StringToDynamicBrushConverterBase("ButtonHoverBackgroundBrushLight", "ButtonHoverBackgroundBrushDark");
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return m_Converter.Convert(value, targetType, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DynamicBrushConverterTaskItem : BaseValueConverter<DynamicBrushConverterTaskItem>
    {
        private readonly StringToDynamicBrushConverterBase m_Converter;

        public DynamicBrushConverterTaskItem()
        {
            m_Converter = new StringToDynamicBrushConverterBase("TaskPageForegroundLightBrush", "TaskPageForegroundDarkBrush");
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush = (SolidColorBrush)m_Converter.Convert(value, targetType, parameter, culture);
            return brush.Color;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DynamicBrushConverterSideMenu : BaseValueConverter<DynamicBrushConverterSideMenu>
    {
        private readonly StringToDynamicBrushConverterBase m_Converter;

        public DynamicBrushConverterSideMenu()
        {
            m_Converter = new StringToDynamicBrushConverterBase("SideMenuForegroundBrush", "SideMenuBackgroundBrush");
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return m_Converter.Convert(value, targetType, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToDynamicBrushConverterBase
    {
        private readonly StringRGBToBrushConverter m_Converter = new StringRGBToBrushConverter();
        private readonly string m_LightBrushResourceName;
        private readonly string m_DarkBrushResourceName;
        private SolidColorBrush m_LightBrush;
        private SolidColorBrush m_DarkBrush;

        public StringToDynamicBrushConverterBase(string lightBrushResourceName, string darkBrushResourceName)
        {
            m_LightBrushResourceName = lightBrushResourceName;
            m_DarkBrushResourceName = darkBrushResourceName;
        }

        public object Convert(object values, Type targetType, object parameter, CultureInfo culture)
        {
            string colorString = (string)values;
            SolidColorBrush colorBrush = (SolidColorBrush)m_Converter.Convert(colorString, targetType, parameter, culture);

            if (colorBrush != null)
            {
                GetBrushes(out m_LightBrush, out m_DarkBrush);
                var brightness = GetBrightness(colorBrush.Color);
                return brightness < 0.5 ? m_LightBrush : m_DarkBrush;
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
            darkBrush = (SolidColorBrush)Application.Current.TryFindResource(m_DarkBrushResourceName);
            lightBrush = (SolidColorBrush)Application.Current.TryFindResource(m_LightBrushResourceName);

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
