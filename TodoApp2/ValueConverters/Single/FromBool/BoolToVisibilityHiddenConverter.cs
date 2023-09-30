using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a boolean and converts it to a visibility enum
    /// </summary>
    public class BoolToVisibilityHiddenConverter : BaseBoolValueConverter<Visibility>
    {
        public bool Negated { get; set; }
        protected override Visibility PositiveValue => Visibility.Visible;
        protected override Visibility NegativeValue => Visibility.Hidden;

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue != Negated)
            {
                return PositiveValue;
            }

            if (value is string stringValue && stringValue.ToLower() == bool.TrueString.ToLower())
            {
                return PositiveValue;
            }

            return NegativeValue;
        }
    }
}