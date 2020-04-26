using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a boolean and converts it to TextDecoration
    /// It is used for task list item description.
    /// </summary>
    public class TextDecorationConverter : BaseValueConverter<TextDecorationConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isDone = (bool)value;

            // Converts the boolean into a TextDecoration
            // The task list item description is strikethrough when the task is done
            return isDone ? TextDecorations.Strikethrough : new TextDecorationCollection();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
