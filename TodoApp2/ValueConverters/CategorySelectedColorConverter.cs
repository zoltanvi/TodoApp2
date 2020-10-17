using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a category name (string) and converts it to a WPF brush
    /// It is used for category list item foreground.
    /// </summary>
    public class CategorySelectedColorConverter : BaseMultiValueConverter<CategorySelectedColorConverter>
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

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string categoryName = (string)values[0];
            string selectedCategoryName = (string)values[1];
            if (selectedCategoryName == categoryName)
            {
                return m_SelectedBrush;
            }
            return m_NormalBrush;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}