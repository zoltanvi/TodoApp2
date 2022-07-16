using System;
using System.Globalization;
using System.Text;

namespace TodoApp2
{
    public class TextToLineNumbersConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(1);
            int count = 1;
            if (value is string text)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == '\n')
                    {
                        sb.AppendLine();
                        sb.Append(++count);
                    }
                }
            }

            return sb.ToString();
        }

    }
}
