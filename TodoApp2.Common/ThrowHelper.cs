using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp2.Common
{
    public static class ThrowHelper
    {
        public static void ThrowIfNull(this object obj, string message = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(message);
            }
        }

        public static void ThrowIfEmpty(this string str, string message = null)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException(message);
            }
        }
    }
}
