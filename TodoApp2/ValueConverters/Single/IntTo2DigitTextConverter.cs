using Modules.Common.Views.ValueConverters;
using System;
using System.Globalization;

namespace TodoApp2;

internal class IntTo2DigitTextConverter : BaseValueConverter
{
    public int MinValue { get; set; } = int.MinValue;

    public int MaxValue { get; set; } = int.MaxValue;

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int number)
        {
            return number.ToString("D2");
        }

        return string.Empty;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string numberText && int.TryParse(numberText, out int convertedInt))
        {
            if (convertedInt < MinValue)
            {
                convertedInt = MinValue;
            }

            if (convertedInt > MaxValue)
            {
                convertedInt = MaxValue;
            }

            return convertedInt;
        }

        return 0;
    }
}
