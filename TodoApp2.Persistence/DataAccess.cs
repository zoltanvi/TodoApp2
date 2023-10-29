using System;
using System.IO;
using TodoApp2.Entity;

namespace TodoApp2.Persistence
{
    public class DataAccess
    {
        private const string GetVersionCommand = "PRAGMA user_version; ";
        public const string DatabaseName = "____TESTDATABASE____.db";
        
        private static string AppDataFolder => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string DatabasePath { get; } = Path.Combine(AppDataFolder, DatabaseName);
        
        public DataAccess()
        {
            string connectionString = $"Data Source={DatabasePath};";
            AppContext appContext = new AppContext(connectionString);
        }
    }
}
