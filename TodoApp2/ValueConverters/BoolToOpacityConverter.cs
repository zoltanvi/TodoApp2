using System;
using System.Globalization;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a boolean and converts it to an opacity value.
    /// It is used in the task list item
    /// </summary>
    public class BoolToOpacityConverter : BaseValueConverter<BoolToOpacityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                // If the item is done
                return 0.5;
            }

            // The item is not done yet
            return 1.0;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
