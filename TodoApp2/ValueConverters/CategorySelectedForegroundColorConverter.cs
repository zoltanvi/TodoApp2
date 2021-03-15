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
    public class CategorySelectedForegroundColorConverter : BaseMultiValueConverter<CategorySelectedForegroundColorConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string categoryName = (string)values[0];
            string selectedCategoryName = (string)values[1];
            if (selectedCategoryName == categoryName)
            {
                return (Brush)Application.Current.TryFindResource("CategoryTitleSelectedForegroundBrush");
            }
            return (Brush)Application.Current.TryFindResource("CategoryTitleForegroundBrush");
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}