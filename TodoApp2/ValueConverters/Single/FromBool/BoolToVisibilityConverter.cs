﻿using System;
using System.Globalization;
using System.Windows;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a boolean and converts it to a visibility enum
    /// </summary>
    public class BoolToVisibilityConverter : BaseBoolValueConverter<Visibility>
    {
        public bool Negated { get; set; }
        protected override Visibility PositiveValue => Visibility.Visible;
        protected override Visibility NegativeValue => Visibility.Collapsed;

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue != Negated)
            {
                return PositiveValue;
            }

            return NegativeValue;
        }
    }
}