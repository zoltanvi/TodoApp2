using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{
    public class BoolToBorderBrushConverter : BaseValueConverter<BoolToBorderBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool valid = (bool)value;

            return valid ? (Brush)Application.Current.TryFindResource("ReminderDateBorderBrush") : (Brush)Application.Current.TryFindResource("ReminderDateInvalidBorderBrush");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}