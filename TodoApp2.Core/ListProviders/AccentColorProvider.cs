using System.Collections.Generic;

namespace TodoApp2.Core
{
    public class AccentColorProvider
    {
        public List<string> Items { get; }

        public AccentColorProvider()
        {
            Items = new List<string>
            {
                "#2E6CAB",
                "#788CDE",
                "#BC7ABC",
                "#E46C8C",
                "#E46B67",
                "#4AA079",
                "#479E98",
                "#8795A0",
                "#A0CBF1",
                "#D6BDE7",
                "#F5B6C2",
            };

        }
    }
}
