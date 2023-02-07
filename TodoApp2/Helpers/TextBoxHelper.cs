using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TodoApp2.Helpers
{
    internal class TextBoxHelper
    {
        public static double GetTextHeight(TextBox textBox, string text)
        {
            Typeface typeFace = new Typeface(
                textBox.FontFamily, 
                textBox.FontStyle, 
                textBox.FontWeight, 
                textBox.FontStretch);

            FormattedText formattedText = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeFace,
                textBox.FontSize,
                Brushes.Black,
                VisualTreeHelper.GetDpi(textBox).PixelsPerDip);

            return formattedText.Height;
        }
    }
}
