using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2
{
    public class BoolToApplicationBorderBrushConverter : BaseValueConverter<BoolToApplicationBorderBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool active = (bool)value;

            return active
                ? Application.Current.TryFindResource("ApplicationBorderBrushActive")
                : Application.Current.TryFindResource("ApplicationBorderBrushInactive");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
