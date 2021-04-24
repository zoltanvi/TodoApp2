using System;
using System.Collections.Generic;
using System.Globalization;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a <see cref="FontSize"/> and converts it to a <see cref="double"/>
    /// </summary>
    public class FontSizeToDoubleConverter : BaseValueConverter<FontSizeToDoubleConverter>
    {
        private const double s_DefaultSize = 16;
        private readonly Dictionary<FontSize, double> m_FontSizes = new Dictionary<FontSize, double>
        {
            {FontSize.Small, 14},
            {FontSize.Medium, s_DefaultSize},
            {FontSize.Big, 20}
        };

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FontSize fValue = (FontSize)value;
            return m_FontSizes.ContainsKey(fValue) ? m_FontSizes[fValue] : s_DefaultSize;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}