namespace TodoApp2.Core.Mappings
{
    internal static class ListOrderParsingHelper
    {
        public static long ParseListOrder(string listOrder)
        {
            if (string.IsNullOrEmpty(listOrder)) return 0L;

            return long.Parse(listOrder);
        }
    }
}
