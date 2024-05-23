using System;
using System.Globalization;

namespace TodoApp2;

public abstract class BaseBoolValueConverter<T> : BaseValueConverter
{
    protected abstract T PositiveValue { get; }
    protected abstract T NegativeValue { get; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && boolValue)
        {
            return PositiveValue;
        }

        return NegativeValue;
    }
}
