using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using TodoApp2.Core;
using MediaFontFamily = System.Windows.Media.FontFamily;

namespace TodoApp2;

/// <summary>
/// A converter that takes in a <see cref="FontFamily"/> and converts it to a <see cref="MediaFontFamily"/>
/// </summary>
public class EnumToFontFamilyConverter : BaseValueConverter
{
    private static bool _initialized = false;
    private static readonly MediaFontFamily DefaultFontFamily = new MediaFontFamily(CoreConstants.FontFamily.SegoeUI);
    private static readonly Dictionary<FontFamily, MediaFontFamily> FontFamilies = new Dictionary<FontFamily, MediaFontFamily>();

    public static EnumToFontFamilyConverter Instance { get; } = new EnumToFontFamilyConverter();

    public EnumToFontFamilyConverter()
    {
        if (!_initialized)
        {
            _initialized = true;
            AddDefaultFontFamily(FontFamily.Calibri, CoreConstants.FontFamily.Calibri);
            AddDefaultFontFamily(FontFamily.Consolas, CoreConstants.FontFamily.Consolas);
            AddDefaultFontFamily(FontFamily.CourierNew, CoreConstants.FontFamily.CourierNew);

            AddFontFamily(FontFamily.FiraSansLight);
            AddFontFamily(FontFamily.FiraSansRegular);

            AddFontFamily(FontFamily.InterLight);
            AddFontFamily(FontFamily.InterRegular);

            AddFontFamily(FontFamily.MontserratAlternatesLight);
            AddFontFamily(FontFamily.MontserratAlternatesRegular);

            AddFontFamily(FontFamily.MontserratLight);
            AddFontFamily(FontFamily.MontserratRegular);

            AddFontFamily(FontFamily.NotoSansLight);
            AddFontFamily(FontFamily.NotoSansRegular);

            AddFontFamily(FontFamily.OpenSansLight);
            AddFontFamily(FontFamily.OpenSans);


            AddDefaultFontFamily(FontFamily.SegoeUILight, CoreConstants.FontFamily.SegoeUILight);
            AddDefaultFontFamily(FontFamily.SegoeUI, CoreConstants.FontFamily.SegoeUI);
            AddDefaultFontFamily(FontFamily.SegoeUIBold, CoreConstants.FontFamily.SegoeUIBold);

            AddDefaultFontFamily(FontFamily.TimesNewRoman, CoreConstants.FontFamily.TimesNewRoman);

            AddFontFamily(FontFamily.UbuntuLight);
            AddFontFamily(FontFamily.UbuntuRegular);

            AddDefaultFontFamily(FontFamily.Verdana, CoreConstants.FontFamily.Verdana);
        }
    }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        FontFamily fValue = (FontFamily)value;
        return FontFamilies.ContainsKey(fValue) ? FontFamilies[fValue] : DefaultFontFamily;
    }

    public MediaFontFamily Convert(object value)
    {
        return (MediaFontFamily)Convert(value, null, null, null);
    }

    private static void AddDefaultFontFamily(FontFamily font, string familyName)
    {
        FontFamilies.Add(font, new MediaFontFamily(familyName));
    }

    private void AddFontFamily(FontFamily font)
    {
        MediaFontFamily fontFamily = (MediaFontFamily)Application.Current.TryFindResource(Enum.GetName(typeof(FontFamily), font));
        FontFamilies.Add(font, fontFamily);
    }
}