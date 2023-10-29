﻿using System;
using System.IO;
using TodoApp2.Entity;

namespace TodoApp2.Persistence
{
    public class DataAccess
    {
        public const string DatabaseName = "000.db";
        
        private static string AppDataFolder => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string DatabasePath { get; } = Path.Combine(AppDataFolder, "WWWTodoDatabase", DatabaseName);
        
        public DataAccess()
        {
            string connectionString = $"Data Source={DatabasePath};";
            //AppContext appContext = new AppContext(connectionString);
        }
    }
}
