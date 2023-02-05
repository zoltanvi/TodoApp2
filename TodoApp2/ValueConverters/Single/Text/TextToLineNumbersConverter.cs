using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace TodoApp2
{
    public class TextToLineNumbersConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StringBuilder sb = new StringBuilder();
            if (value is ObservableCollection<int> numberPositions)
            {
                int number = 1;
                int arrayIndex = 0;
                int lastIndex = numberPositions[numberPositions.Count - 1];
                
                for (int i = 0; i <= lastIndex; i++)
                {
                    if (i == numberPositions[arrayIndex])
                    {
                        arrayIndex++;
                        sb.AppendLine($"{number++}");
                    }
                    else
                    {
                        sb.AppendLine();
                    }
                }
            }

            return sb.ToString();
        }
    }
}
