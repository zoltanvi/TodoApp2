using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp2.Core
{
    public class ThemeListProvider
    {
        public List<Theme> Items { get; }

        public ThemeListProvider()
        {
            Items = Enum.GetValues(typeof(Theme)).Cast<Theme>().ToList();
        }
    }
}
