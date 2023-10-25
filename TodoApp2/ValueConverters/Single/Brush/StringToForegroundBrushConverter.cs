using System;
using System.Globalization;
using System.Windows;
using TodoApp2.Core;

namespace TodoApp2
{
    public class StringToForegroundBrushConverter : BaseValueConverter
    {
        private readonly StringRGBToBrushConverter _converter = new StringRGBToBrushConverter();

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (value is string colorCode && colorCode == GlobalConstants.ColorName.Transparent))
            {
                return Application.Current.TryFindResource(GlobalConstants.BrushName.ForegroundBrush);
            }

            return _converter.Convert(value, targetType, parameter, culture);
        }
    }
}
