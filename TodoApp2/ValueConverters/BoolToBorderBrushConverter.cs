using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2
{
    public class BoolToBorderBrushConverter : BaseValueConverter<BoolToBorderBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool valid = (bool)value;

            return valid
                ? Application.Current.TryFindResource("ReminderDateBorderBrush")
                : Application.Current.TryFindResource("ReminderDateInvalidBorderBrush");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}