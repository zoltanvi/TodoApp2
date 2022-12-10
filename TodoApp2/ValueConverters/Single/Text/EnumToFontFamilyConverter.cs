using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using TodoApp2.Core;
using TodoApp2.Core.Constants;
using MediaFontFamily = System.Windows.Media.FontFamily;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a <see cref="FontFamily"/> and converts it to a <see cref="MediaFontFamily"/>
    /// </summary>
    public class EnumToFontFamilyConverter : BaseValueConverter
    {
        private static readonly MediaFontFamily s_DefaultFontFamily = new MediaFontFamily(GlobalConstants.FontFamily.SegoeUI);
        private static readonly Dictionary<FontFamily, MediaFontFamily> s_FontFamilies = new Dictionary<FontFamily, MediaFontFamily>
        {
            { FontFamily.SegoeUI, s_DefaultFontFamily },
            { FontFamily.Ubuntu, (MediaFontFamily)Application.Current.TryFindResource(GlobalConstants.FontFamily.Ubuntu)},
            { FontFamily.Consolas, new MediaFontFamily(GlobalConstants.FontFamily.Consolas)},
            { FontFamily.Verdana, new MediaFontFamily(GlobalConstants.FontFamily.Verdana)},

            { FontFamily.SourceCodePro, (MediaFontFamily)Application.Current.TryFindResource(GlobalConstants.FontFamily.SourceCodeProRegular)},
            { FontFamily.CascadiaMono, (MediaFontFamily)Application.Current.TryFindResource(GlobalConstants.FontFamily.CascadiaMonoRegular)},
            { FontFamily.CascadiaMonoSemiLight, (MediaFontFamily)Application.Current.TryFindResource(GlobalConstants.FontFamily.CascadiaMonoSemiLight)},
            { FontFamily.CascadiaMonoLight, (MediaFontFamily)Application.Current.TryFindResource(GlobalConstants.FontFamily.CascadiaMonoLight)},

            { FontFamily.Inter, (MediaFontFamily)Application.Current.TryFindResource(GlobalConstants.FontFamily.Inter)},
            { FontFamily.InterLight, (MediaFontFamily)Application.Current.TryFindResource(GlobalConstants.FontFamily.InterLight)},
            { FontFamily.InterBold, (MediaFontFamily)Application.Current.TryFindResource(GlobalConstants.FontFamily.InterBold)},
        };

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FontFamily fValue = (FontFamily)value;
            return s_FontFamilies.ContainsKey(fValue) ? s_FontFamilies[fValue] : s_DefaultFontFamily;
        }

    }
}