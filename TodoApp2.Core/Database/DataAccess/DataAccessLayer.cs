using System;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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

        private readonly SQLiteConnection _connection;

        private const int DatabaseVersion = 6;
        private int _readDatabaseVersion;

        public TaskDataAccess TaskDataAccess { get; }
        public SettingsDataAccess SettingsDataAccess { get; }
        public CategoryDataAccess CategoryDataAccess { get; }
        public NoteDataAccess NoteDataAccess { get; }
        public CompatibilityDataAccess CompatibilityDataAccess { get; }

        public DataAccessLayer()
        {
            _connection = new SQLiteConnection($"Data Source={DatabasePath};");
            _connection.Open();

            TaskDataAccess = new TaskDataAccess(_connection);
            SettingsDataAccess = new SettingsDataAccess(_connection);
            CategoryDataAccess = new CategoryDataAccess(_connection);
            NoteDataAccess = new NoteDataAccess(_connection);
            CompatibilityDataAccess = new CompatibilityDataAccess(_connection);
        }

        public void InitializeDatabase()
        {
            _readDatabaseVersion = CompatibilityDataAccess.ReadDbVersion();

            // Turn foreign keys on
            ExecuteCommand("PRAGMA foreign_keys = ON; ");

            // Create the tables if the DB is empty
            if (!TaskDataAccess.IsTaskTableExists())
            {
                // Update db version
                _readDatabaseVersion = DatabaseVersion;
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
            if (_readDatabaseVersion > DatabaseVersion)
            {
                // Crash the app. The database cannot be used
                throw new Exception("The database file is created by a newer version of the program and could not be read.");
            }

            var dbVersion = _readDatabaseVersion;
            while (dbVersion != DatabaseVersion)
            {
                UpgradeToNextVersion(ref dbVersion);
            }

            // Update db version
            CompatibilityDataAccess.UpdateDbVersion(dbVersion);
        }

        private void UpgradeToNextVersion(ref int currentDbVersion)
        {
            if (currentDbVersion == 0)
            {
                // Missing BorderColor column from Task
                CompatibilityDataAccess.AddBorderColorToTaskTable();
            }
            else if (currentDbVersion == 2)
            {
                // Missing Note table
                NoteDataAccess.CreateNoteTable();
            }
            else if (currentDbVersion == 3)
            {
                // Missing BackgroundColor column from Task
                CompatibilityDataAccess.AddBackgroundColorToTaskTable();
            }
            else if (currentDbVersion == 4)
            {
                // Re-create the settings table
                CompatibilityDataAccess.DropSettingsTable();
                SettingsDataAccess.CreateSettingsTable();
            }
            else if (currentDbVersion == 5)
            {
                // Move TitleBarDateVisible from AppSettings.CommonSettings to AppSettings.DateTimeSettings
                string oldSettingKey = $"{nameof(CommonSettings)}.{nameof(DateTimeSettings.TitleBarDateVisible)}";
                var oldSetting = SettingsDataAccess.GetSetting(oldSettingKey);

                if (oldSetting != null)
                {
                    var newSetting = new Setting($"{nameof(DateTimeSettings)}.{nameof(DateTimeSettings.TitleBarDateVisible)}", oldSetting.Value);
                    SettingsDataAccess.UpdateSetting(oldSettingKey, newSetting);
                }
            }

            currentDbVersion++;
        }

        private void ExecuteCommand(string command)
        {
            using (SQLiteCommand dbCommand = new SQLiteCommand(command, _connection))
            using (SQLiteDataReader reader = dbCommand.ExecuteReader())
            {
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}