﻿using System;
using System.Globalization;

namespace TodoApp2
{
    /// <summary>
    /// A converter that takes in a long and converts it to a formatted date string
    /// </summary>
    public class LongToFormattedLongDateConverter : BaseValueConverter
    {
        // TODO: add to config or make it culture dependent
        private const string DateTimeFormatString = "yyyy. MMMM dd. dddd, HH:mm";

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long dateTicks = (long)value;

            return dateTicks != 0
                ? new DateTime(dateTicks).ToString(DateTimeFormatString)
                : "Set reminder";
        }
    }
}