﻿using System;
using System.Globalization;

namespace TodoApp2
{
    public class LongToTimestampConverter : BaseValueConverter
    {
        // TODO: add to config or make it culture dependent
        private const string DateTimeFormatString = "yyyy-MM-dd HH:mm";

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long dateTicks = (long)value;

            return new DateTime(dateTicks).ToString(DateTimeFormatString);
        }
    }
}
