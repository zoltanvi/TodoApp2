using System.Collections.Generic;

namespace TodoApp2.Core.Helpers
{
    public class NumericStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y)
            {
                return 0;
            }

            // Both strings are numeric; compare them based on their numeric value
            return CompareNumericStrings(x, y);
        }

        private int CompareNumericStrings(string x, string y)
        {
            // Compare based on the sign and then the length
            if (x.StartsWith("-") && !y.StartsWith("-"))
            {
                return -1; // x is negative, y is positive
            }
            if (!x.StartsWith("-") && y.StartsWith("-"))
            {
                return 1; // x is positive, y is negative
            }

            // The order is reversed in case of negative numbers
            if (x.StartsWith("-") && y.StartsWith("-"))
            {
                return x.CompareTo(y) * -1;
            }

            // Both are positive or negative; compare based on length
            if (x.Length < y.Length)
            {
                return -1; // x is shorter
            }
            if (x.Length > y.Length)
            {
                return 1; // x is longer
            }

            // Both are of the same length; compare character by character
            return x.CompareTo(y);
        }
    }
}
