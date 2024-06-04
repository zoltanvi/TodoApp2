using Modules.Common.DataModels;
using Modules.Common.Views.ValueConverters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace TodoApp2;

public class TitleBarHeightConverter : BaseValueConverter
{
    private Dictionary<TitleBarHeight, int> _dictionary = new Dictionary<TitleBarHeight, int>
    {
        { TitleBarHeight.ExtraSlim, 24 },
        { TitleBarHeight.Slim, 28 },
        { TitleBarHeight.Normal, 32 },
        { TitleBarHeight.Tall, 36 },
        { TitleBarHeight.ExtraTall, 42 },
    };

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TitleBarHeight height)
        {
            return _dictionary[height];
        }

        if (value is int intValue)
        {
            return _dictionary.FirstOrDefault(x => x.Value == intValue);
        }

        return TitleBarHeight.Normal;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert(value, targetType, parameter, culture);
    }
}

public class TitleBarHeightToGridLengthConverter : TitleBarHeightConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TitleBarHeight height)
        {
            var number = (int)base.Convert(height, targetType, parameter, culture);
            return new GridLength(number);
        }
     
        return new GridLength(10);
    }
}
