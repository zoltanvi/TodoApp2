using System;
using System.IO;

namespace TodoApp2.Persistence
{
    public static class DataAccess
    {
        public const string DatabaseName = "000.db";
        private static IAppContext Context;
        private static string AppDataFolder => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string ConnectionString = $"Data Source={DatabasePath};";
        public static string DatabasePath { get; } = Path.Combine(AppDataFolder, DatabaseName);

        public static IAppContext GetAppContext() => Context ?? (Context = new AppContext(ConnectionString));
    }
}
