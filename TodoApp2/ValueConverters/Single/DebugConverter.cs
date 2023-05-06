using System;
using System.Globalization;

namespace TodoApp2
{
    public class DebugConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This converter is only used for debugging
            return value;
        }
    }
}
