using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp2.Core
{
    public class EnumValuesListProvider<TEnum> where TEnum : struct
    {
        public List<TEnum> Items { get; }

        public EnumValuesListProvider()
        {
            Items = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
        }
    }
}
