namespace Modules.Common.Extensions;

public static class StringExtensions
{
    public static bool EqualsIgnoreCase(this string? value, string otherValue)
    {
        return string.Equals(value, otherValue, StringComparison.OrdinalIgnoreCase);
    }
}
