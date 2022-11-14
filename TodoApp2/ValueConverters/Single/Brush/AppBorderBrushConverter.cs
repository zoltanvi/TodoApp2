using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{
    internal class AppBorderBrushConverter : BaseValueConverter
    {
        private const string s_DefaultResourceName = "AppBorderBrush";
        private readonly StringRGBToBrushConverter m_Converter = new StringRGBToBrushConverter();

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string brushName)
            {
                if(brushName == "Transparent")
                {
                    return (SolidColorBrush)Application.Current.TryFindResource(s_DefaultResourceName);
                }

                return m_Converter.Convert(value, targetType, parameter, culture);
            }

            return null;
        }
    }
}
