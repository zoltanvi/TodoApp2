using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{
    public class BoolToBorderBrushConverter : BaseValueConverter<BoolToBorderBrushConverter>
    {
        private readonly Brush m_ValidBrush;
        private readonly Brush m_InvalidBrush;
      
        public BoolToBorderBrushConverter()
        {
            m_ValidBrush = (Brush)Application.Current.TryFindResource("ReminderDateBorderBrush");
            m_InvalidBrush = (Brush)Application.Current.TryFindResource("ReminderDateInvalidBorderBrush");
           
            if (m_ValidBrush == null || m_InvalidBrush == null)
            {
                throw new NullReferenceException("Can't find ReminderDate border brush!");
            }
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool valid = (bool)value;

            return valid ? m_ValidBrush : m_InvalidBrush;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
