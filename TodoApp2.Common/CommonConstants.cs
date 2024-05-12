namespace TodoApp2.Common
{
    public static class CommonConstants
    {
        public const int RecycleBinCategoryId = -1000;
        public const long InvalidListOrder = -8999999999999999999L;
        public const long DefaultListOrder = 0;
        public const long ListOrderInterval = 1_000_000_000_000;
        public const string ListOrderFormat = "D19";
        public const string RecycleBinCategoryName = "Recycle bin";
        public static readonly string DefaultListOrderString = FormatListOrder(DefaultListOrder);
        public static string FormatListOrder(this long number) => number.ToString(ListOrderFormat);
    }
}
