using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp2.Core
{
    public class ThemeListService
    {
        public List<Theme> Items { get; }

        public ThemeListService()
        {
            Items = Enum.GetValues(typeof(Theme)).Cast<Theme>().ToList();
        }
    }
}
