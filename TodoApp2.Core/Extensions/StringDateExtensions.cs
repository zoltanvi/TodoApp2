using System;
using System.Text.RegularExpressions;

namespace TodoApp2.Core.Extensions;

public static class StringDateExtensions
{
    /// <summary>
    /// Converts the <see cref="string"/> into a <see cref="DateTime"/>. 
    /// Returns a <see cref="bool"/> value indicating whether the conversion was successful or not.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to convert.</param>
    /// <param name="resultDateTime">The converted <see cref="DateTime"/>. 
    /// If the conversion fails it is <see cref="DateTime.MinValue"/>.</param>
    /// <returns>Returns a <see cref="bool"/> value indicating whether the conversion was successful or not.</returns>
    public static bool ConvertToDate(this string value, out DateTime resultDateTime)
    {
        // Matches: (YEAR)<anything>(MONTH)<anything>(DAY)
        string pattern = @"^(\d{1,4})[^0-9]+?(\d{1,2})[^0-9]+?(\d{1,2})$";
        Match match = Regex.Match(value, pattern);

        int year = 0;
        int month = 0;
        int day = 0;

        if (match.Success)
        {
            year = int.Parse(match.Groups[1].Value);
            month = int.Parse(match.Groups[2].Value);
            day = int.Parse(match.Groups[3].Value);

            resultDateTime = new DateTime(year, month, day);
            return true;
        }

        resultDateTime = DateTime.MinValue;
        return false;
    }

    /// <summary>
    /// Converts the <see cref="DateTime"/> into a <see cref="string"/>. 
    /// </summary>
    /// <param name="value">The <see cref="string"/> to convert.</param>
    public static string ConvertToString(this DateTime value)
    {
        return value.ToString("yyyy-MM-dd");
    }
}
