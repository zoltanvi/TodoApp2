﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using TodoApp2.Core;
using MediaFontFamily = System.Windows.Media.FontFamily;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a <see cref="FontFamily"/> and converts it to a <see cref="MediaFontFamily"/>
    /// </summary>
    public class EnumToBoldFontFamilyConverter : BaseValueConverter<EnumToBoldFontFamilyConverter>
    {
        private static readonly MediaFontFamily s_DefaultFontFamily = new MediaFontFamily("Segoe UI Bold");
        private static readonly Dictionary<FontFamily, MediaFontFamily> s_FontFamilies = new Dictionary<FontFamily, MediaFontFamily>
        {
            { FontFamily.Ubuntu, (MediaFontFamily)Application.Current.TryFindResource("UbuntuBold")},
            { FontFamily.SegoeUI, s_DefaultFontFamily },
            { FontFamily.Consolas, new MediaFontFamily("Consolas Bold")},
            { FontFamily.Verdana, new MediaFontFamily("Verdana Bold")},
    };

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FontFamily fValue = (FontFamily)value;
            return s_FontFamilies.ContainsKey(fValue) ? s_FontFamilies[fValue] : s_DefaultFontFamily;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}