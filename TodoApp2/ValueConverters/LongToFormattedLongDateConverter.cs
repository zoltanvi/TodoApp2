using System;
using System.Globalization;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a long and converts it to a formatted date string
    /// </summary>
    public class LongToFormattedLongDateConverter : BaseValueConverter<LongToFormattedLongDateConverter>
    {
        private const string DateTimeFormatString = "yyyy. MMMM dd. dddd, HH:mm";

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long dateTicks = (long)value;

            return new DateTime(dateTicks).ToString(DateTimeFormatString);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}