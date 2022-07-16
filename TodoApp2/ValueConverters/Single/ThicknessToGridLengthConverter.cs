using System;
using System.Globalization;
using System.Windows;
using Thickness = TodoApp2.Core.Thickness;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a <see cref="TodoApp2.Core.Thickness"/> and converts it to a <see cref="GridLength"/>
    /// </summary>
    public class ThicknessToGridLengthConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GridLength gridLength = new GridLength();

            switch ((Thickness)value)
            {
                case Thickness.VeryThin:
                    gridLength = new GridLength(1);
                    break;
                case Thickness.Thin:
                    gridLength = new GridLength(3);
                    break;
                case Thickness.Medium:
                    gridLength = new GridLength(4);
                    break;
                case Thickness.Thick:
                    gridLength = new GridLength(6);
                    break;
            }

            return gridLength;
        }

    }
}