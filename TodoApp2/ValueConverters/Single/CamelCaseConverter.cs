using System;
using System.Text.RegularExpressions;

namespace TodoApp2;

public class CamelCaseConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        string enumString = value.ToString();
        string res = string.Join(" ", Regex.Split(enumString, "(?<!(^|[A-Z]))(?=[A-Z])|(?<!^)(?=[A-Z][a-z])"));
        return res;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return value;
    }
}
