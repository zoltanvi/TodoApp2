namespace TodoApp2.Common
{
    public static class CommonConstants
    {
        public const long InvalidListOrder = -9000000000000000000L;
        public const long MaxListOrder = 9000000000000000000L;
        public const int DefaultListOrder = 0;
        public const long ListOrderInterval = 1_000_000_000_000;
        public const string ListOrderFormat = "D19";
        public static readonly string DefaultListOrderString = FormatListOrder(DefaultListOrder);
        public static string FormatListOrder(this long number) => number.ToString(ListOrderFormat);
    }
}
