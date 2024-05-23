using System;
using System.Collections.Generic;
using System.Globalization;
using TodoApp2.Core;

namespace TodoApp2;

public class HorizontalAlignmentConverter : BaseValueConverter
{
    private Dictionary<HorizontalAlignment, System.Windows.HorizontalAlignment> _dictionary = new Dictionary<HorizontalAlignment, System.Windows.HorizontalAlignment>
    {
        { HorizontalAlignment.Left, System.Windows.HorizontalAlignment.Left },
        { HorizontalAlignment.Center, System.Windows.HorizontalAlignment.Center },
        { HorizontalAlignment.Right, System.Windows.HorizontalAlignment.Right }
    };

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is HorizontalAlignment alignment)
        {
            return _dictionary[alignment];
        }

        return System.Windows.HorizontalAlignment.Left;
    }
}

public class HorizontalAlignmentInvertedConverter : BaseValueConverter
{
    private Dictionary<HorizontalAlignment, System.Windows.HorizontalAlignment> _dictionary = new Dictionary<HorizontalAlignment, System.Windows.HorizontalAlignment>
    {
        { HorizontalAlignment.Left, System.Windows.HorizontalAlignment.Right },
        { HorizontalAlignment.Center, System.Windows.HorizontalAlignment.Right },
        { HorizontalAlignment.Right, System.Windows.HorizontalAlignment.Left }
    };

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is HorizontalAlignment alignment)
        {
            return _dictionary[alignment];
        }

        return System.Windows.HorizontalAlignment.Left;
    }
}
