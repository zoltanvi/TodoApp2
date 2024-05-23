using System;
using System.Globalization;

namespace TodoApp2;

public class CurrentDateTimeFormatMultiConverter : BaseMultiValueConverter<CurrentDateTimeFormatMultiConverter>
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {

        if (values.Length == 2 && values[0] is long ticks && values[1] is string dateFormat)
        {
            try
            {
                return new DateTime(ticks).ToString(dateFormat);
            }
            catch (Exception)
            {
                // Ignore
            }
        }
     
        return "INVALID CurrentDateTimeFormatMultiConverter";
    }
}
