using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2
{
    public class BoolToBorderThicknessConverter : BaseValueConverter<BoolToBorderThicknessConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool)value;
            return new Thickness(val ? 0 : 9);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
