using Modules.Common.DataModels;
using Modules.Common.Views.ValueConverters;
using System;
using System.Globalization;

namespace TodoApp2;

/// <summary>
/// A converter that takes in a <see cref="Modules.Common.DataModels.Thickness"/> and converts it to a <see cref="double"/>
/// </summary>
public class ThicknessToDoubleConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double thickness = 0;
        switch ((Thickness)value)
        {
            case Thickness.VeryThin:
                thickness = 1;
                break;
            case Thickness.Thin:
                thickness = 3;
                break;
            case Thickness.Medium:
                thickness = 5;
                break;
            case Thickness.Thick:
                thickness = 8;
                break;
            case Thickness.ExtraThick:
                thickness = 15;
                break;
            case Thickness.ExtremelyThick:
                thickness = 20;
                break;
        }

        return thickness;
    }

}
