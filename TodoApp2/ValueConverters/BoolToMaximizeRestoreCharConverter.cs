using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2
{
    public class BoolToMaximizeRestoreCharConverter : BaseValueConverter<BoolToMaximizeRestoreCharConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string maximizeButtonContent = (string)Application.Current.TryFindResource("SegoeIconMaximizeWindow2");
            
            if (value is bool isMaximized && isMaximized)
            {
                maximizeButtonContent = (string)Application.Current.TryFindResource("SegoeIconRestoreWindow");
            }

            return maximizeButtonContent;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
