using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a category name (string) and converts it to a WPF brush
    /// It is used for category list item background.
    /// </summary>
    public class CategorySelectedColorConverter : BaseMultiValueConverter<CategorySelectedColorConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string categoryName = (string)values[0];
            string selectedCategoryName = (string)values[1];
            if (selectedCategoryName == categoryName)
            {
                return (Brush)Application.Current.TryFindResource("CategorySelectedBackgroundBrush");
            }
            return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            //return (Brush)Application.Current.TryFindResource("SideMenuBackgroundBrush");
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}