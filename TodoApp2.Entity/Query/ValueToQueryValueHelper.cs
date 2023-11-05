using System;

namespace TodoApp2.Entity.Query
{
    internal static class ValueToQueryValueHelper
    {
        public static string FormatValue(object value, Type type)
        {
            if (type == typeof(bool)) return value.ToString();
            else if (type == typeof(int)) return value.ToString();
            else if (type == typeof(long)) return value.ToString();
            else if (type == typeof(string)) return $"'{value}'";

            throw new ArgumentException($"Unknown type! Value = {value}, type = {type}.");
        }
    }
}
