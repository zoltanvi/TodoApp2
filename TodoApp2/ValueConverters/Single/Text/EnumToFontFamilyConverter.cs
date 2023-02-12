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
        private static readonly Dictionary<FontFamily, MediaFontFamily> s_FontFamilies = new Dictionary<FontFamily, MediaFontFamily>();

        public EnumToFontFamilyConverter()
        {
            AddDefaultFontFamily(FontFamily.Calibri, GlobalConstants.FontFamily.Calibri);
            
            AddFontFamily(FontFamily.CascadiaMonoLight);
            AddFontFamily(FontFamily.CascadiaMonoRegular);
            AddFontFamily(FontFamily.CascadiaMonoBold);

            AddDefaultFontFamily(FontFamily.Consolas, GlobalConstants.FontFamily.Consolas);

            AddFontFamily(FontFamily.CormorantLight);
            AddFontFamily(FontFamily.CormorantRegular);
            AddFontFamily(FontFamily.CormorantBold);

            AddDefaultFontFamily(FontFamily.CourierNew, GlobalConstants.FontFamily.CourierNew);

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

            AddDefaultFontFamily(FontFamily.SegoeUI, GlobalConstants.FontFamily.SegoeUI);

            AddFontFamily(FontFamily.SourceCodeProRegular);
            AddFontFamily(FontFamily.SourceCodeProBold);

            AddDefaultFontFamily(FontFamily.Tahoma, GlobalConstants.FontFamily.Tahoma);
            AddDefaultFontFamily(FontFamily.TimesNewRoman, GlobalConstants.FontFamily.TimesNewRoman);

            AddFontFamily(FontFamily.UbuntuLight);
            AddFontFamily(FontFamily.UbuntuRegular);
            AddFontFamily(FontFamily.UbuntuBold);

            AddDefaultFontFamily(FontFamily.Verdana, GlobalConstants.FontFamily.Verdana);
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FontFamily fValue = (FontFamily)value;
            return s_FontFamilies.ContainsKey(fValue) ? s_FontFamilies[fValue] : s_DefaultFontFamily;
        }

        private static void AddDefaultFontFamily(FontFamily font, string familyName)
        {
            s_FontFamilies.Add(font, new MediaFontFamily(familyName));
        }

        private void AddFontFamily(FontFamily font)
        {
            MediaFontFamily fontFamily = (MediaFontFamily)Application.Current.TryFindResource(Enum.GetName(typeof(FontFamily), font));
            s_FontFamilies.Add(font, fontFamily);
        }
    }
}