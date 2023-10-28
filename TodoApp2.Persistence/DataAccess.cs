using System;
using System.IO;
using TodoApp2.Entity;

namespace TodoApp2.Persistence
{
    public class DataAccess
    {
        public const string DatabaseName = "____TESTDATABASE____.db";
        private static string AppDataFolder => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string DatabasePath { get; } = Path.Combine(AppDataFolder, DatabaseName);
        private static string connectionString { get; } = $"Data Source={DatabasePath};";

        private const string GetVersionCommand = "PRAGMA user_version; ";

        public DataAccess()
        {
            AppContext appContext = new AppContext(connectionString);
            
            //var settings = appContext.Settings.GetAll();
            //var notes = appContext.Notes.GetAll();
            //var tasks = appContext.Tasks.GetAll();
            //var categories = appContext.Categories.GetAll();
        }
    }
}
