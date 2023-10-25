using System;
using System.Globalization;
using TodoApp2.Core;

namespace TodoApp2
{

    internal class TaskSpacingToMarginConverter : BaseValueConverter
    {
        TaskSpacingToDoubleConverter _converter = new TaskSpacingToDoubleConverter();

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double marginThickness = 0; 
            
            if (value is TaskSpacing taskSpacing)
            {
                marginThickness = (double)_converter.Convert(taskSpacing, targetType, parameter, culture);
            }

            return new System.Windows.Thickness(0, marginThickness, 0, marginThickness);
        }
    }
}