using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using TodoApp2.Core;

namespace TodoApp2
{
    internal abstract class DefaultBorderBrushConverter : BaseValueConverter
    {
        private readonly StringRGBToBrushConverter _converter = new StringRGBToBrushConverter();

        protected abstract string DefaultResourceName { get; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string brushName)
            {
                if (brushName == GlobalConstants.ColorName.Transparent)
                {
                    return (SolidColorBrush)Application.Current.TryFindResource(DefaultResourceName);
                }

                return _converter.Convert(value, targetType, parameter, culture);
            }

            return null;
        }
    }
}
