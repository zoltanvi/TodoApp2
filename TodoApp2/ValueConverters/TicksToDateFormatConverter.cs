using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{

    public class TicksToDateFormatConverter : BaseValueConverter<TicksToDateFormatConverter>
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long dateInTicks = (long)value;

            DateTime date = new DateTime(dateInTicks);

            // Converts the boolean into a brush
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
