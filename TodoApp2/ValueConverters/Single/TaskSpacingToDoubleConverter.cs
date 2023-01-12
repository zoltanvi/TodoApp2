using System;
using System.Globalization;
using TodoApp2.Core;

namespace TodoApp2
{
    internal class TaskSpacingToDoubleConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double height = 10;

            if (value is TaskSpacing taskSpacing)
            {
                switch (taskSpacing)
                {
                    case TaskSpacing.Compact:
                        height = 10;
                        break;

                    case TaskSpacing.Normal:
                        height = 40;
                        break;

                    case TaskSpacing.Comfortable:
                        height = 60;
                        break;

                    case TaskSpacing.Spacious:
                        height = 80;
                        break;
                }
            }

            return height;
        }
    }
}