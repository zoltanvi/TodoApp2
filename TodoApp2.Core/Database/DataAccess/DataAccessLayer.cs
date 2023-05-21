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

        private const int DatabaseVersion = 4;
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
                CreateShortcutTasks();
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
                    break;
                }
                case 2:
                {
                    // Missing Note table
                    NoteDataAccess.CreateNoteTable();

                    // Missing BackgroundColor column from Task
                    CompatibilityDataAccess.AddBackgroundColorToTaskTable();
                    break;
                }
                case 3:
                {
                    // Missing BackgroundColor column from Task
                    CompatibilityDataAccess.AddBackgroundColorToTaskTable();
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

        private void CreateShortcutTasks()
        {
            string globalShortcuts = "<FlowDocument PagePadding=\"5,0,5,0\" AllowDrop=\"True\" NumberSubstitution.CultureSource=\"User\" " +
                "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" +
                "<Paragraph><Run FontStyle=\"Italic\" FontWeight=\"Bold\" FontSize=\"22\" Foreground=\"#FF42BDA8\" >" +
                "Text editor shortcuts</Run><LineBreak /><Run Foreground=\"#FF42BDA8\" >Ctrl + B</Run>" +
                "<Run xml:space=\"preserve\">\t\t</Run><Run FontWeight=\"Bold\" >Bold</Run>" +
                "<LineBreak /><Run Foreground=\"#FF42BDA8\" xml:space=\"preserve\">Ctrl + U\t</Run>" +
                "<Run xml:space=\"preserve\">\t</Run><Run ><Run.TextDecorations>" +
                "<TextDecoration Location=\"Underline\" /></Run.TextDecorations>Underlined</Run><LineBreak /><Run Foreground=\"#FF42BDA8\" " +
                " xml:space=\"preserve\">Ctrl + I\t</Run><Run xml:space=\"preserve\">\tI</Run>" +
                "<Run FontStyle=\"Italic\" >talic</Run><LineBreak /><Run Foreground=\"#FF42BDA8\" " +
                "xml:space=\"preserve\" /><LineBreak /><Run Foreground=\"#FF42BDA8\" >Ctrl + Space</Run>" +
                "<Run xml:space=\"preserve\">\tRemove format (selection)</Run><LineBreak /><Run Foreground=\"#FF42BDA8\" " +
                ">Ctrl + H</Run><Run xml:space=\"preserve\">\t\tRemove format (all)</Run><LineBreak />" +
                "<Run Foreground=\"#FF42BDA8\" xml:space=\"preserve\" /><LineBreak /><Run Foreground=\"#FF42BDA8\"" +
                " xml:space=\"preserve\">Ctrl + L\t</Run><Run xml:space=\"preserve\">\tAlign left</Run>" +
                "<LineBreak /><Run Foreground=\"#FF42BDA8\" >Ctrl + E</Run><Run xml:space=\"preserve\">" +
                "\t\tAlign center</Run><LineBreak /><Run Foreground=\"#FF42BDA8\" >Ctrl + R</Run><Run " +
                "xml:space=\"preserve\">\t\tAlign right</Run><LineBreak /><Run Foreground=\"#FF42BDA8\" >Ctrl + J</Run>" +
                "<Run xml:space=\"preserve\">\t\tJustify</Run></Paragraph><Paragraph><Run Foreground=\"#FF42BDA8\" " +
                " xml:space=\"preserve\" /><LineBreak /><Run Foreground=\"#FF42BDA8\" >Ctrl + [</Run>" +
                "<Run xml:space=\"preserve\">\t\tDecrease font size</Run><LineBreak /><Run Foreground=\"#FF42BDA8\" " +
                ">Ctrl + ]</Run><Run xml:space=\"preserve\">\t\tIncrease font size</Run><LineBreak />" +
                "<Run Foreground=\"#FF42BDA8\" xml:space=\"preserve\" /><LineBreak /><Run Foreground=\"#FF42BDA8\" " +
                " xml:space=\"preserve\">Ctrl + =\t</Run><Run xml:space=\"preserve\">" +
                "\tSubscript ( e.g. x</Run><Run Typography.Variants=\"Subscript\">4</Run><Run " +
                "xml:space=\"preserve\"> )</Run><LineBreak /><Run Foreground=\"#FF42BDA8\" >Ctrl + Shift + =</Run>" +
                "<Run xml:space=\"preserve\"> \tSuperscript ( e.g. x</Run><Run " +
                "Typography.Variants=\"Superscript\" xml:space=\"preserve\">4 </Run><Run >)</Run></Paragraph>" +
                "</FlowDocument>";

            string textEditorShortcuts = "<FlowDocument PagePadding=\"5,0,5,0\" AllowDrop=\"True\" NumberSubstitution.CultureSource=\"User\"" +
                " xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" +
                "<Paragraph><Run FontStyle=\"Italic\" FontWeight=\"Bold\" FontSize=\"22\" Foreground=\"#FF42BDA8\" >Global shortcuts</Run>" +
                "<LineBreak /><Run Foreground=\"#FF42BDA8\" >Ctrl + Shift + J</Run><Run xml:space=\"preserve\">\tPrevious theme</Run>" +
                "<LineBreak /><Run Foreground=\"#FF42BDA8\" >Ctrl + Shift + L</Run><Run xml:space=\"preserve\">\tNext theme</Run><LineBreak />" +
                "<LineBreak /><Run Foreground=\"#FF42BDA8\" xml:space=\"preserve\">Ctrl + Z\t</Run><Run xml:space=\"preserve\">" +
                "\tUndo (Add / Delete task)</Run><LineBreak /><Run Foreground=\"#FF42BDA8\" xml:space=\"preserve\">Ctrl + Y\t</Run>" +
                "<Run xml:space=\"preserve\">\tRedo (Add / Delete task)</Run><LineBreak /><Run Foreground=\"#FFEC407A\" xml:space=\"preserve\" />" +
                "<LineBreak /><Run Foreground=\"#FF42BDA8\" >Ctrl + Scroll ↑</Run><Run xml:space=\"preserve\">\tZoom in</Run><LineBreak />" +
                "<Run Foreground=\"#FF42BDA8\" xml:space=\"preserve\">Ctrl + '+'\t</Run><Run xml:space=\"preserve\">\tZoom in</Run><LineBreak />" +
                "<Run Foreground=\"#FFEC407A\" xml:space=\"preserve\" /><LineBreak /><Run Foreground=\"#FF42BDA8\" >Ctrl + Scroll ↓</Run>" +
                "<Run xml:space=\"preserve\">\tZoom out</Run><LineBreak /><Run Foreground=\"#FF42BDA8\" xml:space=\"preserve\">Ctrl + '-'\t</Run>" +
                "<Run xml:space=\"preserve\">\tZoom out</Run></Paragraph></FlowDocument>";

            TaskDataAccess.CreateTask(textEditorShortcuts, 1);
            TaskDataAccess.CreateTask(globalShortcuts, 1);
        }

        public void Dispose()
        {
            m_Connection?.Close();
            m_Connection?.Dispose();
        }
    }
}