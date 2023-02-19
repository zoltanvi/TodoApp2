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

        private const int DatabaseVersion = 3;
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

                if (CategoryDataAccess.AddDefaultCategoryIfNotExists())
                {
                    CreateShortcutTasks();
                }

                SettingsDataAccess.AddDefaultSettingsIfNotExists();
            }

            UpgradeToCurrentVersion();
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
                    break;
                }
                case 2:
                {
                    // Missing Note table
                    NoteDataAccess.CreateNoteTable();
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
                "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" " +
                "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"><Paragraph><Run FontWeight=\"Bold\" FontSize=\"22\" " +
                "xml:lang=\"hu-hu\"><Run.TextDecorations><TextDecoration Location=\"Underline\" /></Run.TextDecorations>" +
                "Global shortcuts</Run><LineBreak /><Run Foreground=\"#FFEC407A\" xml:lang=\"hu-hu\">Ctrl + Shift + J</Run>" +
                "<Run xml:lang=\"hu-hu\" xml:space=\"preserve\">\tPrevious theme</Run><LineBreak /><Run Foreground=\"#FFEC407A\" " +
                "xml:lang=\"hu-hu\">Ctrl + Shift + L</Run><Run xml:lang=\"hu-hu\" xml:space=\"preserve\">\tNext theme</Run>" +
                "<LineBreak /><Run Foreground=\"#FFEC407A\" xml:lang=\"hu-hu\">Ctrl + Z</Run><Run xml:lang=\"hu-hu\" " +
                "xml:space=\"preserve\">\t\tUndo (Add / Delete task)</Run><LineBreak /><Run Foreground=\"#FFEC407A\" " +
                "xml:lang=\"hu-hu\">Ctrl + Y</Run><Run xml:lang=\"hu-hu\" xml:space=\"preserve\">\t\tRedo (Add / Delete task)" +
                "</Run><LineBreak /><Run Foreground=\"#FFEC407A\" xml:lang=\"hu-hu\">Ctrl + Scroll ↑</Run><Run xml:lang=\"hu-hu\" " +
                "xml:space=\"preserve\">\tZoom in</Run><LineBreak /><Run Foreground=\"#FFEC407A\" xml:lang=\"hu-hu\">Ctrl + Scroll ↓" +
                "</Run><Run xml:lang=\"hu-hu\" xml:space=\"preserve\">\tZoom in</Run></Paragraph></FlowDocument>";

            string textEditorShortcuts = "<FlowDocument PagePadding=\"5,0,5,0\" AllowDrop=\"True\" NumberSubstitution.CultureSource=\"User\" " +
                "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" +
                "<Paragraph><Run FontWeight=\"Bold\" FontSize=\"22\" xml:lang=\"hu-hu\"><Run.TextDecorations><TextDecoration " +
                "Location=\"Underline\" /></Run.TextDecorations>Text editor shortcuts</Run><LineBreak /><Run Foreground=\"#FFEC407A\" " +
                "xml:lang=\"hu-hu\">Ctrl + B</Run><Run xml:lang=\"hu-hu\" xml:space=\"preserve\">\t\t</Run><Run FontWeight=\"Bold\" " +
                "xml:lang=\"hu-hu\">Bold</Run><LineBreak /><Run Foreground=\"#FFEC407A\" xml:lang=\"hu-hu\">Ctrl + U</Run>" +
                "<Run xml:lang=\"hu-hu\" xml:space=\"preserve\">\t\t</Run><Run xml:lang=\"hu-hu\"><Run.TextDecorations><TextDecoration " +
                "Location=\"Underline\" /></Run.TextDecorations>Underlined</Run><LineBreak /><Run Foreground=\"#FFEC407A\" " +
                "xml:lang=\"hu-hu\">Ctrl + I</Run><Run xml:lang=\"hu-hu\" xml:space=\"preserve\">\t\tI</Run><Run FontStyle=\"Italic\" " +
                "xml:lang=\"hu-hu\">talic</Run><LineBreak /><Run Foreground=\"#FFEC407A\" xml:lang=\"hu-hu\">Ctrl + Space</Run><Run " +
                "xml:lang=\"hu-hu\" xml:space=\"preserve\">\tRemove formatting</Run><LineBreak /><Run Foreground=\"#FFEC407A\" " +
                "xml:lang=\"hu-hu\">Ctrl + L</Run><Run xml:lang=\"hu-hu\" xml:space=\"preserve\">\t\tAlign left</Run><LineBreak />" +
                "<Run Foreground=\"#FFEC407A\" xml:lang=\"hu-hu\">Ctrl + E</Run><Run xml:lang=\"hu-hu\" xml:space=\"preserve\">\t\tAlign center" +
                "</Run><LineBreak /><Run Foreground=\"#FFEC407A\" xml:lang=\"hu-hu\">Ctrl + R</Run><Run xml:lang=\"hu-hu\" " +
                "xml:space=\"preserve\">\t\tAlign right</Run><LineBreak /><Run Foreground=\"#FFEC407A\" xml:lang=\"hu-hu\">Ctrl + J</Run>" +
                "<Run xml:lang=\"hu-hu\" xml:space=\"preserve\">\t\tJustify</Run></Paragraph><Paragraph><Run Foreground=\"#FFEC407A\" " +
                "xml:lang=\"hu-hu\">Ctrl + [</Run><Run xml:lang=\"hu-hu\" xml:space=\"preserve\">\t\tDecrease font size</Run><LineBreak />" +
                "<Run Foreground=\"#FFEC407A\" xml:lang=\"hu-hu\">Ctrl + ]</Run><Run xml:lang=\"hu-hu\" xml:space=\"preserve\">\t\t" +
                "Increase font size</Run><LineBreak /><Run Foreground=\"#FFEC407A\" xml:lang=\"hu-hu\">Ctrl + =</Run><Run xml:lang=\"hu-hu\" " +
                "xml:space=\"preserve\">\t\tSubscript ( e.g. x</Run><Run xml:lang=\"hu-hu\" Typography.Variants=\"Subscript\">4</Run>" +
                "<Run xml:lang=\"hu-hu\" xml:space=\"preserve\"> )</Run><LineBreak /><Run Foreground=\"#FFEC407A\" xml:lang=\"hu-hu\">" +
                "Ctrl + Shift + =</Run><Run xml:lang=\"hu-hu\" xml:space=\"preserve\"> \tSuperscript ( e.g. x</Run><Run xml:lang=\"hu-hu\" " +
                "Typography.Variants=\"Superscript\" xml:space=\"preserve\">4 </Run><Run xml:lang=\"hu-hu\">)</Run></Paragraph></FlowDocument>";

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