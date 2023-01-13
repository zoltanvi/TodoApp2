using System;
using System.Globalization;
using TodoApp2.Core;

namespace TodoApp2
{
    internal class TaskSpacingToDoubleConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double height = 2;

            if (value is TaskSpacing taskSpacing)
            {
                switch (taskSpacing)
                {
                    case TaskSpacing.Compact:
                        height = 2;
                        break;

                    case TaskSpacing.Normal:
                        height = 6;
                        break;

                    case TaskSpacing.Comfortable:
                        height = 10;
                        break;

                    case TaskSpacing.Spacious:
                        height = 20;
                        break;
                }
            }

            return height;
        }
    }
}