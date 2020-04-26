using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.SQLite;
using TodoApp2.Core.ViewModel;

namespace TodoApp2.Core
{
    /// <summary>
    /// The data access layer to access information from the database
    /// </summary>
    public class DataAccess
    {
        #region Private Constants

        private const string s_DatabaseName = "TodoAppDatabase.db";
        private static string DatabasePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), s_DatabaseName);

        private const string s_TableCategory = "Category";
        private const string s_TableReminder = "Reminder";
        private const string s_TableTask = "Task";

        private const string s_FieldName = "Name";
        private const string s_FieldId = "Id";
        private const string s_FieldReminderDate = "ReminderDate";
        private const string s_FieldNote = "Note";
        private const string s_FieldCategory = "Category";
        private const string s_FieldContent = "Content";
        private const string s_FieldListOrder = "ListOrder";
        private const string s_FieldIsDone = "IsDone";
        private const string s_FieldCreationDate = "CreationDate";
        private const string s_FieldModificationDate = "ModificationDate";
        private const string s_FieldColor = "Color";
        private const string s_FieldTrashed = "Trashed";
        private const string s_FieldReminderId = "ReminderId";

        private const string s_ParameterFieldName = "@" + s_FieldName;
        private const string s_ParameterFieldId = "@" + s_FieldId;
        private const string s_ParameterFieldReminderDate = "@" + s_FieldReminderDate;
        private const string s_ParameterFieldNote = "@" + s_FieldNote;
        private const string s_ParameterFieldCategory = "@" + s_FieldCategory;
        private const string s_ParameterFieldContent = "@" + s_FieldContent;
        private const string s_ParameterFieldListOrder = "@" + s_FieldListOrder;
        private const string s_ParameterFieldIsDone = "@" + s_FieldIsDone;
        private const string s_ParameterFieldCreationDate = "@" + s_FieldCreationDate;
        private const string s_ParameterFieldModificationDate = "@" + s_FieldModificationDate;
        private const string s_ParameterFieldColor = "@" + s_FieldColor;
        private const string s_ParameterFieldTrashed = "@" + s_FieldTrashed;
        private const string s_ParameterFieldReminderId = "@" + s_FieldReminderId;
        #endregion

        #region Database initializer

        public static void InitializeDatabase()
        {
            if (!File.Exists(DatabasePath))
            {
                FileStream fs = File.Create(DatabasePath);
                fs.Close();
            }

            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                const string prepareCommand = "PRAGMA foreign_keys = ON; ";

                const string createCategoryTable =
                    "CREATE TABLE IF NOT EXISTS " + s_TableCategory + " ( " +
                    s_FieldName + "              TEXT        PRIMARY KEY " +
                    "  ); ";
                const string createReminderTable =
                    "CREATE TABLE IF NOT EXISTS " + s_TableReminder + " ( " +
                    s_FieldId + "               INTEGER     PRIMARY KEY AUTOINCREMENT, " +
                    s_FieldReminderDate + "     INTEGER     , " +
                    s_FieldNote + "             TEXT         " +
                    "  ); ";
                const string createTaskTable =
                    "CREATE TABLE IF NOT EXISTS " + s_TableTask + " ( " +
                    s_FieldId + "               INTEGER     PRIMARY KEY AUTOINCREMENT, " +
                    s_FieldCategory + "         TEXT        REFERENCES " + s_TableCategory + "(" + s_FieldName + ") ON UPDATE CASCADE, " +
                    s_FieldContent + "          TEXT        , " +
                    s_FieldListOrder + "        INTEGER     , " +
                    s_FieldIsDone + "           INTEGER     , " +
                    s_FieldCreationDate + "     INTEGER     , " +
                    s_FieldModificationDate + " INTEGER     , " +
                    s_FieldColor + "            TEXT        , " +
                    s_FieldTrashed + "          INTEGER     , " +
                    s_FieldReminderId + "       INTEGER     REFERENCES " + s_TableReminder + "(" + s_FieldId + ") ON UPDATE CASCADE " +
                    "  ); ";

                // Prepare database
                SQLiteCommand dbCommand = new SQLiteCommand(prepareCommand, db);
                dbCommand.ExecuteReader();

                // Create CATEGORY table
                dbCommand = new SQLiteCommand(createCategoryTable, db);
                dbCommand.ExecuteReader();

                // Create REMINDER table
                dbCommand = new SQLiteCommand(createReminderTable, db);
                dbCommand.ExecuteReader();

                // Create TASK table
                dbCommand = new SQLiteCommand(createTaskTable, db);
                dbCommand.ExecuteReader();

                db.Close();
            }

            AddCategoryIfNotExists("Default");
            AddDefaultReminderIfNotExists();
        }

        #endregion

        #region Add methods

        public static bool AddCategoryIfNotExists(string categoryName)
        {
            if (GetCategories().All(x => x.Name.ToLowerInvariant() != categoryName.ToLowerInvariant()))
            {
                AddCategory(categoryName);
                return true;
            }

            return false;
        }

        public static bool AddDefaultReminderIfNotExists()
        {
            if (GetReminders().All(x => x.Id != 1))
            {
                AddReminder(0, string.Empty);
                return true;
            }

            return false;
        }

        public static void AddReminder(long reminderDate, string note)
        {
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                SQLiteCommand insertCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = $"INSERT INTO {s_TableReminder} ({s_FieldReminderDate}, {s_FieldNote}) " +
                                  $"VALUES ({s_ParameterFieldReminderDate}, {s_ParameterFieldNote});"
                };

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.Parameters.AddWithValue(s_ParameterFieldReminderDate, reminderDate);
                insertCommand.Parameters.AddWithValue(s_ParameterFieldNote, note);

                insertCommand.ExecuteReader();

                db.Close();
            }
        }


        public static void AddCategory(string name)
        {
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                SQLiteCommand insertCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = $"INSERT INTO {s_TableCategory} ({s_FieldName}) VALUES ({s_ParameterFieldName});"
                };

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.Parameters.AddWithValue("@name", name);

                insertCommand.ExecuteReader();

                db.Close();
            }
        }

        /// <summary>
        /// Returns the auto generated id of the added task.
        /// </summary>
        /// <param name="taskListItem"></param>
        /// <returns></returns>
        public static int AddTask(TaskListItemViewModel taskListItem)
        {
            int taskId = -1;
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                SQLiteCommand insertCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = $"INSERT INTO {s_TableTask} " +
                                  $" ({s_FieldCategory}, {s_FieldContent}, {s_FieldListOrder}, " +
                                  $" {s_FieldIsDone}, {s_FieldCreationDate}, {s_FieldModificationDate}, " +
                                  $" {s_FieldColor}, {s_FieldTrashed}, {s_FieldReminderId}) " +
                                  $"VALUES ({s_ParameterFieldCategory}, {s_ParameterFieldContent}, " +
                                  $" {s_ParameterFieldListOrder}, {s_ParameterFieldIsDone}, " +
                                  $" {s_ParameterFieldCreationDate}, {s_ParameterFieldModificationDate}, " +
                                  $" {s_ParameterFieldColor}, {s_ParameterFieldTrashed}, " +
                                  $" {s_ParameterFieldReminderId});"
                };

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.Parameters.AddWithValue($"{s_ParameterFieldCategory}", taskListItem.Category);
                insertCommand.Parameters.AddWithValue($"{s_ParameterFieldContent}", taskListItem.Content);
                insertCommand.Parameters.AddWithValue($"{s_ParameterFieldListOrder}", taskListItem.ListOrder);
                insertCommand.Parameters.AddWithValue($"{s_ParameterFieldIsDone}", taskListItem.IsDone);
                insertCommand.Parameters.AddWithValue($"{s_ParameterFieldCreationDate}", taskListItem.CreationDate);
                insertCommand.Parameters.AddWithValue($"{s_ParameterFieldModificationDate}", taskListItem.ModificationDate);
                insertCommand.Parameters.AddWithValue($"{s_ParameterFieldColor}", taskListItem.Color);
                insertCommand.Parameters.AddWithValue($"{s_ParameterFieldTrashed}", taskListItem.Trashed);
                insertCommand.Parameters.AddWithValue($"{s_ParameterFieldReminderId}", taskListItem.ReminderId);

                insertCommand.ExecuteReader();

                SQLiteCommand selectCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = "SELECT last_insert_rowid();"
                };

                SQLiteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    taskId = query.GetInt32(0);
                }

                db.Close();
            }

            return taskId;
        }

        #endregion

        #region Get methods

        public static List<CategoryViewModel> GetCategories()
        {
            List<CategoryViewModel> entries = new List<CategoryViewModel>();

            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                SQLiteCommand selectCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = $"SELECT {s_FieldName} FROM {s_TableCategory}"
                };

                SQLiteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(new CategoryViewModel { Name = query.GetString(0) });
                }

                db.Close();
            }

            return entries;
        }

        public static List<ReminderViewModel> GetReminders()
        {
            List<ReminderViewModel> entries = new List<ReminderViewModel>();

            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                SQLiteCommand selectCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = $"SELECT * FROM {s_TableReminder}"
                };

                SQLiteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(
                        new ReminderViewModel
                        {
                            Id = query.GetInt32(query.GetOrdinal(s_FieldId)),
                            ReminderDate = query.GetInt64(query.GetOrdinal(s_FieldReminderDate)),
                            Note = query.GetString(query.GetOrdinal(s_FieldNote)),
                        });
                }

                db.Close();
            }

            return entries;
        }

        public static List<TaskListItemViewModel> GetTasks(string category)
        {
            List<TaskListItemViewModel> entries = new List<TaskListItemViewModel>();

            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                SQLiteCommand selectCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = $"SELECT * " +
                                  $" FROM {s_TableTask} " +
                                  $" WHERE {s_TableTask}.{s_FieldCategory} IS {s_ParameterFieldCategory};"
                };

                // Use parameterized query to prevent SQL injection attacks
                selectCommand.Parameters.AddWithValue(s_ParameterFieldCategory, category);

                SQLiteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    TaskListItemViewModel readTask = new TaskListItemViewModel
                    {
                        Id = query.GetInt32(query.GetOrdinal(s_FieldId)),
                        Category = query.GetString(query.GetOrdinal(s_FieldCategory)),
                        Content = query.GetString(query.GetOrdinal(s_FieldContent)),
                        ListOrder = query.GetInt32(query.GetOrdinal(s_FieldListOrder)),
                        IsDone = Convert.ToBoolean(query.GetInt32(query.GetOrdinal(s_FieldIsDone))),
                        CreationDate = query.GetInt64(query.GetOrdinal(s_FieldCreationDate)),
                        ModificationDate = query.GetInt64(query.GetOrdinal(s_FieldModificationDate)),
                        Color = query.GetString(query.GetOrdinal(s_ParameterFieldColor)),
                        Trashed = Convert.ToBoolean(query.GetInt32(query.GetOrdinal(s_FieldTrashed))),
                        ReminderId = query.GetInt32(query.GetOrdinal(s_FieldReminderId))
                    };

                    entries.Add(readTask);
                }

                db.Close();
            }

            return entries;
        }

        #endregion

        #region Delete methods

        public static bool DeleteTask(int id)
        {
            bool isOperationSuccessful;
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                SQLiteCommand deleteCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = $"DELETE FROM {s_TableTask} WHERE {s_FieldId} = {s_ParameterFieldId}"
                };
                // Use parameterized query to prevent SQL injection attacks
                deleteCommand.Parameters.AddWithValue(s_ParameterFieldId, id);

                isOperationSuccessful = deleteCommand.ExecuteNonQuery() > 0;

                db.Close();
            }

            return isOperationSuccessful;
        }

        public static bool DeleteCategory(string name)
        {
            bool isOperationSuccessful;
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                SQLiteCommand deleteCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = $"DELETE FROM {s_TableCategory} " +
                                  $" WHERE {s_FieldName} IS {s_ParameterFieldCategory}"
                };

                // Use parameterized query to prevent SQL injection attacks
                deleteCommand.Parameters.AddWithValue(s_ParameterFieldCategory, name);

                isOperationSuccessful = deleteCommand.ExecuteNonQuery() > 0;

                db.Close();
            }

            return isOperationSuccessful;
        }

        #endregion

        #region Update methods

        public static bool UpdateTask(TaskListItemViewModel task)
        {
            bool isOperationSuccessful;
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                SQLiteCommand updateCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = $"UPDATE {s_TableTask} SET " +
                                  $"  {s_FieldCategory} = {s_ParameterFieldCategory}, " +
                                  $"  {s_FieldContent} = {s_ParameterFieldContent}, " +
                                  $"  {s_FieldListOrder} = {s_ParameterFieldListOrder}, " +
                                  $"  {s_FieldIsDone} = {s_ParameterFieldIsDone}, " +
                                  $"  {s_FieldCreationDate} = {s_ParameterFieldCreationDate}, " +
                                  $"  {s_FieldModificationDate} = {s_ParameterFieldModificationDate}, " +
                                  $"  {s_FieldColor} = {s_ParameterFieldColor}, " +
                                  $"  {s_FieldTrashed} = {s_ParameterFieldTrashed}, " +
                                  $"  {s_FieldReminderId} = {s_ParameterFieldReminderId} " +
                                  $" WHERE {s_FieldId} = {s_ParameterFieldId};"
                };

                // Use parameterized query to prevent SQL injection attacks
                updateCommand.Parameters.AddWithValue(s_ParameterFieldId, task.Id);
                updateCommand.Parameters.AddWithValue(s_ParameterFieldCategory, task.Category);
                updateCommand.Parameters.AddWithValue(s_ParameterFieldContent, task.Content);
                updateCommand.Parameters.AddWithValue(s_ParameterFieldListOrder, task.ListOrder);
                updateCommand.Parameters.AddWithValue(s_ParameterFieldIsDone, task.IsDone);
                updateCommand.Parameters.AddWithValue(s_ParameterFieldCreationDate, task.CreationDate);
                updateCommand.Parameters.AddWithValue(s_ParameterFieldModificationDate, task.ModificationDate);
                updateCommand.Parameters.AddWithValue(s_ParameterFieldColor, task.Color);
                updateCommand.Parameters.AddWithValue(s_ParameterFieldTrashed, task.Trashed);
                updateCommand.Parameters.AddWithValue(s_ParameterFieldReminderId, task.ReminderId);

                isOperationSuccessful = updateCommand.ExecuteNonQuery() > 0;

                db.Close();
            }

            return isOperationSuccessful;
        }

        public static int UpdateTaskList(IEnumerable<TaskListItemViewModel> taskList)
        {
            int modifiedItems = 0;
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                foreach (var task in taskList)
                {
                    SQLiteCommand updateCommand = new SQLiteCommand
                    {
                        Connection = db,
                        CommandText = $"UPDATE {s_TableTask} SET " +
                                      $"  {s_FieldCategory} = {s_ParameterFieldCategory}, " +
                                      $"  {s_FieldContent} = {s_ParameterFieldContent}, " +
                                      $"  {s_FieldListOrder} = {s_ParameterFieldListOrder}, " +
                                      $"  {s_FieldIsDone} = {s_ParameterFieldIsDone}, " +
                                      $"  {s_FieldCreationDate} = {s_ParameterFieldCreationDate}, " +
                                      $"  {s_FieldModificationDate} = {s_ParameterFieldModificationDate}, " +
                                      $"  {s_FieldColor} = {s_ParameterFieldColor}, " +
                                      $"  {s_FieldTrashed} = {s_ParameterFieldTrashed}, " +
                                      $"  {s_FieldReminderId} = {s_ParameterFieldReminderId} " +
                                      $" WHERE {s_FieldId} = {s_ParameterFieldId};"
                    };

                    // Use parameterized query to prevent SQL injection attacks
                    updateCommand.Parameters.AddWithValue(s_ParameterFieldId, task.Id);
                    updateCommand.Parameters.AddWithValue(s_ParameterFieldCategory, task.Category);
                    updateCommand.Parameters.AddWithValue(s_ParameterFieldContent, task.Content);
                    updateCommand.Parameters.AddWithValue(s_ParameterFieldListOrder, task.ListOrder);
                    updateCommand.Parameters.AddWithValue(s_ParameterFieldIsDone, task.IsDone);
                    updateCommand.Parameters.AddWithValue(s_ParameterFieldCreationDate, task.CreationDate);
                    updateCommand.Parameters.AddWithValue(s_ParameterFieldModificationDate, task.ModificationDate);
                    updateCommand.Parameters.AddWithValue(s_ParameterFieldColor, task.Color);
                    updateCommand.Parameters.AddWithValue(s_ParameterFieldTrashed, task.Trashed);
                    updateCommand.Parameters.AddWithValue(s_ParameterFieldReminderId, task.ReminderId);

                    modifiedItems += updateCommand.ExecuteNonQuery();
                }

                db.Close();
            }

            return modifiedItems;
        }


        public static bool UpdateCategory(string oldName, string newName)
        {
            bool isOperationSuccessful;
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                SQLiteCommand updateCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = $"UPDATE {s_TableCategory} SET " +
                                  $" {s_FieldName} = {s_ParameterFieldName}" +
                                  $"WHERE {s_FieldName} IS @oldName;"
                };

                // Use parameterized query to prevent SQL injection attacks
                updateCommand.Parameters.AddWithValue("@oldName", oldName);
                updateCommand.Parameters.AddWithValue(s_ParameterFieldName, newName);

                isOperationSuccessful = updateCommand.ExecuteNonQuery() > 0;

                db.Close();
            }

            return isOperationSuccessful;
        }

        public static int UpdateTaskListOrders(IEnumerable<TaskListItemViewModel> taskList)
        {
            int modifiedItems = 0;
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                foreach (var todoTask in taskList)
                {
                    SQLiteCommand updateCommand = new SQLiteCommand
                    {
                        Connection = db,
                        CommandText = $"UPDATE {s_TableTask} SET " +
                                      $"  {s_FieldListOrder} = {s_ParameterFieldListOrder} " +
                                      $"WHERE {s_FieldId} = {s_ParameterFieldId};"
                    };
                    // Use parameterized query to prevent SQL injection attacks
                    updateCommand.Parameters.AddWithValue(s_ParameterFieldId, todoTask.Id);
                    updateCommand.Parameters.AddWithValue(s_ParameterFieldListOrder, todoTask.ListOrder);

                    modifiedItems += updateCommand.ExecuteNonQuery();
                }

                db.Close();
            }

            return modifiedItems;
        }

        #endregion

        /// <summary>
        /// Creates a database file.  This just creates a zero-byte file which SQLite
        /// will turn into a database when the file is opened properly.
        /// </summary>
        /// <param name="databaseFileName">The file to create</param>
        private static void CreateFile(string databaseFileName)
        {
            FileStream fs = File.Create(databaseFileName);
            fs.Close();
        }

    }
}
