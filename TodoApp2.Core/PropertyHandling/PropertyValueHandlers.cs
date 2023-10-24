using System;
using System.Globalization;

namespace TodoApp2.Core
{
    public class BoolPropertyValueHandler : BasePropertyValueHandler<bool>
    {
        protected override bool TryParseValue(string value, out bool result) =>
            bool.TryParse(value, out result);
    }

    public class DoublePropertyValueHandler : BasePropertyValueHandler<double>
    {
        protected override bool TryParseValue(string value, out double result) =>
            double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);

        protected override string DataToString(double value) => 
            value.ToString(CultureInfo.InvariantCulture);
    }

    public class IntegerPropertyValueHandler : BasePropertyValueHandler<int>
    {
        protected override bool TryParseValue(string value, out int result) =>
            int.TryParse(value, out result);
    }

    public class EnumPropertyValueHandler<TEnum> : BasePropertyValueHandler<TEnum> 
        where TEnum : struct
    {
        protected override bool TryParseValue(string value, out TEnum result) =>
            Enum.TryParse(value, out result);
    }

    public class StringPropertyValueHandler : BasePropertyValueHandler<string>
    {
        protected override bool TryParseValue(string value, out string result)
        {
            result = value;
            return true;
        }
    }
}