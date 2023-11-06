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
            
                AddFontFamily(FontFamily.CascadiaMonoLight);
                AddFontFamily(FontFamily.CascadiaMonoRegular);
                AddFontFamily(FontFamily.CascadiaMonoBold);

                AddDefaultFontFamily(FontFamily.Consolas, CoreConstants.FontFamily.Consolas);

                AddFontFamily(FontFamily.CormorantLight);
                AddFontFamily(FontFamily.CormorantRegular);
                AddFontFamily(FontFamily.CormorantBold);

                AddDefaultFontFamily(FontFamily.CourierNew, CoreConstants.FontFamily.CourierNew);

                AddFontFamily(FontFamily.FiraSansLight);
                AddFontFamily(FontFamily.FiraSansRegular);
                AddFontFamily(FontFamily.FiraSansBold);

                AddFontFamily(FontFamily.InterLight);
                AddFontFamily(FontFamily.InterRegular);
                AddFontFamily(FontFamily.InterBold);

                AddFontFamily(FontFamily.MontserratAlternatesLight);
                AddFontFamily(FontFamily.MontserratAlternatesRegular);
                AddFontFamily(FontFamily.MontserratAlternatesBold);

                AddFontFamily(FontFamily.MontserratLight);
                AddFontFamily(FontFamily.MontserratRegular);
                AddFontFamily(FontFamily.MontserratBold);

                AddFontFamily(FontFamily.NotoSansLight);
                AddFontFamily(FontFamily.NotoSansRegular);
                AddFontFamily(FontFamily.NotoSansBold);

                AddFontFamily(FontFamily.SCE_PS3);

                AddDefaultFontFamily(FontFamily.SegoeUILight, CoreConstants.FontFamily.SegoeUILight);
                AddDefaultFontFamily(FontFamily.SegoeUI, CoreConstants.FontFamily.SegoeUI);
                AddDefaultFontFamily(FontFamily.SegoeUIBold, CoreConstants.FontFamily.SegoeUIBold);

                AddFontFamily(FontFamily.SourceCodeProRegular);
                AddFontFamily(FontFamily.SourceCodeProBold);

                AddDefaultFontFamily(FontFamily.Tahoma, CoreConstants.FontFamily.Tahoma);
                AddDefaultFontFamily(FontFamily.TimesNewRoman, CoreConstants.FontFamily.TimesNewRoman);

                AddFontFamily(FontFamily.UbuntuLight);
                AddFontFamily(FontFamily.UbuntuRegular);
                AddFontFamily(FontFamily.UbuntuBold);

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
}