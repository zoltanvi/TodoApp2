using System;
using System.Globalization;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a <see cref="TodoApp2.Core.Thickness"/> and converts it to a <see cref="double"/>
    /// </summary>
    public class ThicknessToDoubleConverter : BaseValueConverter<ThicknessToDoubleConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double thickness = 0;
            switch ((Thickness)value)
            {
                case Thickness.Thin:
                    thickness = 3;
                    break;
                case Thickness.Thick:
                    thickness = 6;
                    break;
            }

            return thickness;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
