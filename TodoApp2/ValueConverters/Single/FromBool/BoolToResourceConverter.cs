using Modules.Common.Views.ValueConverters;
using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2;

public class BoolToResourceConverter : BaseValueConverter
{
    public string PositiveResourceName { get; set; }
    public string NegativeResourceName { get; set; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool valid = (bool)value;

        return valid
            ? Application.Current.TryFindResource(PositiveResourceName)
            : Application.Current.TryFindResource(NegativeResourceName);
    }
}