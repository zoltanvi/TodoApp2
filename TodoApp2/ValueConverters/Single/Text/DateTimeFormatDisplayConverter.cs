using System;
using System.Globalization;

namespace TodoApp2;

public class DateTimeFormatDisplayConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string dateTimeFormat)
        {
            try
            {
                return DateTime.Now.ToString(dateTimeFormat);
            }
            catch (Exception)
            {
                // Ignore
            }
        }

        return DateTime.Now.ToString();
    }
}
