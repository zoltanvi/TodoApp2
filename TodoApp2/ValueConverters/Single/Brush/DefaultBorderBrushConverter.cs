using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{
    internal abstract class DefaultBorderBrushConverter : BaseValueConverter
    {
        private readonly StringRGBToBrushConverter m_Converter = new StringRGBToBrushConverter();

        protected abstract string DefaultResourceName { get; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string brushName)
            {
                if (brushName == "Transparent")
                {
                    return (SolidColorBrush)Application.Current.TryFindResource(DefaultResourceName);
                }

                return m_Converter.Convert(value, targetType, parameter, culture);
            }

            return null;
        }
    }
}
