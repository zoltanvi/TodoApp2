namespace TodoApp2.Persistence
{
    internal static class Constants
    {
        public static string DefaultListOrder { get; } = (long.MaxValue / 2).ToString();
        public const string Transparent = "Transparent";
        public const string Zero = "0";

        public static class TableNames
        {
            public const string Task = "Task";
            public const string Category = "Category";
            public const string Note = "Note";
            public const string Setting = "Setting";
        }
    }
}
