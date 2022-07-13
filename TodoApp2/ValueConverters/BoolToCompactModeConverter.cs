using System;
using System.Globalization;

namespace TodoApp2
{
    public class BoolToCompactModeConverter : BaseValueConverter<BoolToCompactModeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool boolValue && boolValue)
            {
                return 10;
            }

            return 40;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
