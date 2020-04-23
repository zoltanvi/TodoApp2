﻿using System;
using System.Globalization;
using System.Windows.Media;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in an RGB string such as FF00FF and converts it to a WPF brush
    /// </summary>
    public class StringRGBToBrushConverter : BaseValueConverter<StringRGBToBrushConverter>
    {
        private const string s_TransparentColor = "Transparent";

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Prefixes the input string with a # character, except if it is "Transparent"
            string inputColor = ((string)value == s_TransparentColor ? string.Empty : "#") + value;
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(inputColor));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
