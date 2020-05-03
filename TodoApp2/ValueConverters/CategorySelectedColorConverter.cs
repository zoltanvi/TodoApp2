using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a bool and converts it to a WPF brush
    /// It is used for category list item foreground.
    /// </summary>
    public class CategorySelectedColorConverter : BaseValueConverter<CategorySelectedColorConverter>
    {
        private readonly Brush m_NormalBrush;
        private readonly Brush m_SelectedBrush;

        public CategorySelectedColorConverter()
        {
           m_NormalBrush = (Brush)Application.Current.TryFindResource("CategoryTitleForegroundBrush");
           m_SelectedBrush = (Brush)Application.Current.TryFindResource("CategoryTitleSelectedForegroundBrush");
           
           if (m_NormalBrush == null || m_SelectedBrush == null)
           {
               throw new NullReferenceException("Can't find Category background brush!");
           }
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isSelected = (bool)value;

            // Converts the isSelected property into a brush
            // If the current category is selected, then the selected brush is returned
            return isSelected ? m_SelectedBrush : m_NormalBrush;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
