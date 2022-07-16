using System;
using System.Globalization;

namespace TodoApp2
{
    public class BoolToDoubleConverter : BaseValueConverter
    {
        public double PositiveValue { get; set; }
        public double NegativeValue { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool boolValue && boolValue)
            {
                return PositiveValue;
            }

            return NegativeValue;
        }
    }
}
