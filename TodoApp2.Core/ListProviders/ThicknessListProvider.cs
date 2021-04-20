using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp2.Core
{
    public class ThicknessListProvider
    {
        public List<Thickness> Items { get; }

        public ThicknessListProvider()
        {
            Items = Enum.GetValues(typeof(Thickness)).Cast<Thickness>().ToList();
        }
    }
}
