using System;
using System.Collections.Generic;
using System.Globalization;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a <see cref="FontSize"/> and converts it to a <see cref="double"/>
    /// </summary>
    public class FontSizeToDoubleConverter : BaseValueConverter
    {
        private const double s_DefaultSize = 16;
        private static readonly Dictionary<FontSize, double> s_FontSizes = new Dictionary<FontSize, double>
        {
            {FontSize.Tiny, 12},
            {FontSize.Small, 14},
            {FontSize.Medium, s_DefaultSize},
            {FontSize.Large, 18},
            {FontSize.Huge, 20}
        };

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FontSize fValue = (FontSize)value;
            return (s_FontSizes.ContainsKey(fValue) ? s_FontSizes[fValue] : s_DefaultSize) * UIScaler.StaticScaleValue;
        }
    }
}