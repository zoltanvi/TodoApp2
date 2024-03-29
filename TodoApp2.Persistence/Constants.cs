﻿namespace TodoApp2.Persistence
{
    internal static class Constants
    {
        public const string Transparent = "Transparent";
        public const string Zero = "0";
        public const string DatabaseFileExtension = "db";

        public static class TableNames
        {
            public const string Task = "Task";
            public const string Category = "Category";
            public const string Note = "Note";
            public const string Setting = "Setting";
        }

        // Keys in the App.config
        public static class ConfigKeys
        {
            public const string DatabaseDirectoryPath = "DatabaseDirectoryPath";
            public const string DatabaseFileName = "DatabaseFileName";
        }
    }
}
