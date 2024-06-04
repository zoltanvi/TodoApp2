using Modules.Common;
using Modules.Common.DataModels;
using Modules.Common.Views.ValueConverters;
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
    private static readonly MediaFontFamily DefaultFontFamily = new MediaFontFamily(Constants.FontFamily.SegoeUI);
    private static readonly Dictionary<FontFamily, MediaFontFamily> FontFamilies = new Dictionary<FontFamily, MediaFontFamily>();

    public static EnumToFontFamilyConverter Instance { get; } = new EnumToFontFamilyConverter();

    public EnumToFontFamilyConverter()
    {
        if (!_initialized)
        {
            _initialized = true;
            AddDefaultFontFamily(FontFamily.Calibri, Constants.FontFamily.Calibri);
            AddDefaultFontFamily(FontFamily.Consolas, Constants.FontFamily.Consolas);
            AddDefaultFontFamily(FontFamily.CourierNew, Constants.FontFamily.CourierNew);

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


            AddDefaultFontFamily(FontFamily.SegoeUILight, Constants.FontFamily.SegoeUILight);
            AddDefaultFontFamily(FontFamily.SegoeUI, Constants.FontFamily.SegoeUI);
            AddDefaultFontFamily(FontFamily.SegoeUIBold, Constants.FontFamily.SegoeUIBold);

            AddDefaultFontFamily(FontFamily.TimesNewRoman, Constants.FontFamily.TimesNewRoman);

            AddFontFamily(FontFamily.UbuntuLight);
            AddFontFamily(FontFamily.UbuntuRegular);

            AddDefaultFontFamily(FontFamily.Verdana, Constants.FontFamily.Verdana);
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