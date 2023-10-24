using System;
using System.Data.SQLite;
using System.IO;

namespace TodoApp2.Core
{
    /// <summary>
    /// The data access layer to access information from the database
    /// </summary>
    public sealed class DataAccessLayer : IDisposable
    {
        public const long DefaultListOrder = BaseDataAccess.DefaultListOrder;
        public const long ListOrderInterval = BaseDataAccess.ListOrderInterval;

        private static string AppDataFolder => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        
        public const string DatabaseName = "TodoApp2Database.db";
        public static string DatabasePath { get; } = Path.Combine(AppDataFolder, DatabaseName);

        private readonly SQLiteConnection m_Connection;

        private const int DatabaseVersion = 5;
        private int m_ReadDatabaseVersion;

        public TaskDataAccess TaskDataAccess { get; }
        public SettingsDataAccess SettingsDataAccess { get; }
        public CategoryDataAccess CategoryDataAccess { get; }
        public NoteDataAccess NoteDataAccess { get; }
        public CompatibilityDataAccess CompatibilityDataAccess { get; }

        public DataAccessLayer()
        {
            m_Connection = new SQLiteConnection($"Data Source={DatabasePath};");
            m_Connection.Open();

            TaskDataAccess = new TaskDataAccess(m_Connection);
            SettingsDataAccess = new SettingsDataAccess(m_Connection);
            CategoryDataAccess = new CategoryDataAccess(m_Connection);
            NoteDataAccess = new NoteDataAccess(m_Connection);
            CompatibilityDataAccess = new CompatibilityDataAccess(m_Connection);
        }

        public void InitializeDatabase()
        {
            m_ReadDatabaseVersion = CompatibilityDataAccess.ReadDbVersion();

            // Turn foreign keys on
            ExecuteCommand("PRAGMA foreign_keys = ON; ");

            // Create the tables if the DB is empty
            if (!TaskDataAccess.IsTaskTableExists())
            {
                // Update db version
                m_ReadDatabaseVersion = DatabaseVersion;
                CompatibilityDataAccess.UpdateDbVersion(DatabaseVersion);

                SettingsDataAccess.CreateSettingsTable();
                CategoryDataAccess.CreateCategoryTable();
                TaskDataAccess.CreateTaskTable();
                NoteDataAccess.CreateNoteTable();

                SettingsDataAccess.AddDefaultSettingsIfNotExists();
            }

            UpgradeToCurrentVersion();
        }

        public void AddDefaultRecords()
        {
            if (!CategoryDataAccess.AddDefaultCategoryIfNotExists())
            {
                // Add default tasks for demonstration
            }
        }

        private void UpgradeToCurrentVersion()
        {
            if (m_ReadDatabaseVersion > DatabaseVersion)
            {
                // Crash the app. The database cannot be used
                throw new Exception("The database file is created by a newer version of the program and could not be read.");
            }

            switch (m_ReadDatabaseVersion)
            {
                case 0:
                {
                    // Missing BorderColor column from Task
                    CompatibilityDataAccess.AddBorderColorToTaskTable();

                    // Missing Note table
                    NoteDataAccess.CreateNoteTable();

                    // Missing BackgroundColor column from Task
                    CompatibilityDataAccess.AddBackgroundColorToTaskTable();

                    // Re-create the settings table
                    CompatibilityDataAccess.DropSettingsTable();
                    SettingsDataAccess.CreateSettingsTable();

                    break;
                }
                case 2:
                {
                    // Missing Note table
                    NoteDataAccess.CreateNoteTable();

                    // Missing BackgroundColor column from Task
                    CompatibilityDataAccess.AddBackgroundColorToTaskTable();

                    // Re-create the settings table
                    CompatibilityDataAccess.DropSettingsTable();
                    SettingsDataAccess.CreateSettingsTable();
                
                    break;
                }
                case 3:
                {
                    // Missing BackgroundColor column from Task
                    CompatibilityDataAccess.AddBackgroundColorToTaskTable();

                    // Re-create the settings table
                    CompatibilityDataAccess.DropSettingsTable();
                    SettingsDataAccess.CreateSettingsTable();

                    break;
                }
                case 4:
                {
                    // Re-create the settings table
                    CompatibilityDataAccess.DropSettingsTable();
                    SettingsDataAccess.CreateSettingsTable();

                    break;
                }
            }

            // Update db version
            CompatibilityDataAccess.UpdateDbVersion(DatabaseVersion);
        }

        private void ExecuteCommand(string command)
        {
            using (SQLiteCommand dbCommand = new SQLiteCommand(command, m_Connection))
            using (SQLiteDataReader reader = dbCommand.ExecuteReader())
            {
            }
        }

        public void Dispose()
        {
            m_Connection?.Close();
            m_Connection?.Dispose();
        }
    }
}