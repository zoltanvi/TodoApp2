using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp2
{
    public class StringToDateConverter : BaseValueConverter<StringToDateConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            if(date == DateTime.MinValue)
            {
                date = DateTime.Now;
            }
            return date.ToString("yyyy-MM-dd");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringDate = (string)value;
            DateTime.TryParse(stringDate, out DateTime resultDateTime);
            return resultDateTime;
        }
    }
}
