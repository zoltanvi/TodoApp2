using System;
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
    public class EnumToFontFamilyConverter : BaseValueConverter
    {
        private static readonly MediaFontFamily s_DefaultFontFamily = new MediaFontFamily("Segoe UI");
        private static readonly Dictionary<FontFamily, MediaFontFamily> s_FontFamilies = new Dictionary<FontFamily, MediaFontFamily>
        {
            { FontFamily.SegoeUI, s_DefaultFontFamily },
            { FontFamily.Ubuntu, (MediaFontFamily)Application.Current.TryFindResource("Ubuntu")},
            { FontFamily.Consolas, new MediaFontFamily("Consolas")},
            { FontFamily.Verdana, new MediaFontFamily("Verdana")},

            { FontFamily.SourceCodePro, (MediaFontFamily)Application.Current.TryFindResource("SourceCodeProRegular")},
            { FontFamily.CascadiaMono, (MediaFontFamily)Application.Current.TryFindResource("CascadiaMonoRegular")},
            { FontFamily.CascadiaMonoSemiLight, (MediaFontFamily)Application.Current.TryFindResource("CascadiaMonoSemiLight")},
            { FontFamily.CascadiaMonoLight, (MediaFontFamily)Application.Current.TryFindResource("CascadiaMonoLight")},

            { FontFamily.Inter, (MediaFontFamily)Application.Current.TryFindResource("Inter")},
            { FontFamily.InterLight, (MediaFontFamily)Application.Current.TryFindResource("InterLight")},
            { FontFamily.InterBold, (MediaFontFamily)Application.Current.TryFindResource("InterBold")},
        };

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FontFamily fValue = (FontFamily)value;
            return s_FontFamilies.ContainsKey(fValue) ? s_FontFamilies[fValue] : s_DefaultFontFamily;
        }

    }
}