using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2
{
    internal class RoundedCornersMultiConverter : BaseMultiValueConverter<RoundedCornersMultiConverter>
    {
        const double CornerRadius = 8;

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is bool turnedOn && values[1] is bool allowed)
            {
                if (turnedOn && allowed)
                {
                    return new CornerRadius(CornerRadius);
                }
            }

            return new CornerRadius(0);
        }
    }
}
