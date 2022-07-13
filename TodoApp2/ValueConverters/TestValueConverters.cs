using System;
using System.Globalization;
using System.Windows.Media;

namespace TodoApp2
{
    public class TextRenderingModeConverter : BaseValueConverter<TextRenderingModeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string renderingMode)
            {
                string lowerCase = renderingMode.ToLower();

                switch (lowerCase)
                {
                    case "auto":
                    case "0":
                        return TextRenderingMode.Auto;
                    case "cleartype":
                    case "1":
                        return TextRenderingMode.ClearType;
                    case "grayscale":
                    case "2":
                        return TextRenderingMode.Grayscale;
                    case "aliased":
                    case "3":
                        return TextRenderingMode.Aliased;
                }
            }

            return TextRenderingMode.Auto;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TextFormattingModeConverter : BaseValueConverter<TextFormattingModeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool formattingMode && formattingMode)
            {
                return TextFormattingMode.Display;
            }

            return TextFormattingMode.Ideal;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ClearTypeHintConverter : BaseValueConverter<ClearTypeHintConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool clearTypeValue && clearTypeValue)
            {
                return ClearTypeHint.Enabled;
            }

            return ClearTypeHint.Auto;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
