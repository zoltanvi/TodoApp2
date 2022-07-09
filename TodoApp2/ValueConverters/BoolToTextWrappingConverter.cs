using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TodoApp2
{
    public class BoolToTextWrappingConverter : BaseValueConverter<BoolToTextWrappingConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool needsWrapping && needsWrapping)
            {
                return TextWrapping.Wrap;
            }

            return TextWrapping.NoWrap;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
