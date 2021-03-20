using System;
using System.Globalization;
using System.Linq;

namespace TodoApp2
{
    public class TaskContextMenuCommandParameterConverter : BaseMultiValueConverter<TaskContextMenuCommandParameterConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.ToList();
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
