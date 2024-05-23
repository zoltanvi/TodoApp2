using System;
using System.Globalization;
using TodoApp2.Core;

namespace TodoApp2;

public class CategoryIdToCategoryNameConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int categoryId)
        {
            CategoryViewModel category = IoC.CategoryListService.GetCategory(categoryId);
            return category.Name;
        }

        return "Invalid conversion";
    }
}
