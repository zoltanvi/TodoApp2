using System;

namespace TodoApp2.Core
{
    public static class StringDateConverterExtender
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
            bool returnValue = false;
            if (DateTime.TryParse(value, out resultDateTime))
            {
                returnValue = true;
            }

            return returnValue;
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
}
