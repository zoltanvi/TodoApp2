using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.SQLite;

namespace TodoApp2.Core
{
    /// <summary>
    /// The data access layer to access information from the database
    /// </summary>
    public static class DataAccessLayer
    {
        #region Private Constants

        private const string DatabaseName = "TodoApp2Database.db";
        private static string DatabasePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), DatabaseName);

        private const string Reminder = "Reminder";
        private const string Task = "Task";

        private const string Name = "Name";
        private const string Id = "Id";
        private const string ReminderDate = "ReminderDate";
        private const string Note = "Note";
        private const string Category = "Category";
        private const string Content = "Content";
        private const string ListOrder = "ListOrder";
        private const string IsDone = "IsDone";
        private const string CreationDate = "CreationDate";
        private const string ModificationDate = "ModificationDate";
        private const string Color = "Color";
        private const string Trashed = "Trashed";
        private const string ReminderId = "ReminderId";


        private const string ParameterName = "@" + Name;
        private const string ParameterId = "@" + Id;
        private const string ParameterReminderDate = "@" + ReminderDate;
        private const string ParameterNote = "@" + Note;
        private const string ParameterCategory = "@" + Category;
        private const string ParameterContent = "@" + Content;
        private const string ParameterListOrder = "@" + ListOrder;
        private const string ParameterIsDone = "@" + IsDone;
        private const string ParameterCreationDate = "@" + CreationDate;
        private const string ParameterModificationDate = "@" + ModificationDate;
        private const string ParameterColor = "@" + Color;
        private const string ParameterTrashed = "@" + Trashed;
        private const string ParameterReminderId = "@" + ReminderId;
        private const string ParameterOldName = "@oldName";
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
                    "CREATE TABLE IF NOT EXISTS " + Category + " ( " +
                    Name + "              TEXT        PRIMARY KEY " +
                    "  ); ";
                const string createReminderTable =
                    "CREATE TABLE IF NOT EXISTS " + Reminder + " ( " +
                    Id + "               INTEGER     PRIMARY KEY AUTOINCREMENT, " +
                    ReminderDate + "     INTEGER     , " +
                    Note + "             TEXT         " +
                    "  ); ";
                const string createTaskTable =
                    "CREATE TABLE IF NOT EXISTS " + Task + " ( " +
                    Id + "               INTEGER     PRIMARY KEY AUTOINCREMENT, " +
                    Category + "         TEXT        REFERENCES " + Category + "(" + Name + ") ON UPDATE CASCADE, " +
                    Content + "          TEXT        , " +
                    ListOrder + "        INTEGER     , " +
                    IsDone + "           INTEGER     , " +
                    CreationDate + "     INTEGER     , " +
                    ModificationDate + " INTEGER     , " +
                    Color + "            TEXT        , " +
                    Trashed + "          INTEGER     , " +
                    ReminderId + "       INTEGER     REFERENCES " + Reminder + "(" + Id + ") ON UPDATE CASCADE " +
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

            AddDefaultReminderIfNotExists();
        }

        #endregion

        #region Add methods

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
                    CommandText = $"INSERT INTO {Reminder} ({ReminderDate}, {Note}) " +
                                  $"VALUES ({ParameterReminderDate}, {ParameterNote});",
                    Parameters =
                    {
                        new SQLiteParameter(ParameterReminderDate, reminderDate),
                        new SQLiteParameter(ParameterNote, note),
                    }
                };

                insertCommand.ExecuteReader();

                db.Close();
            }
        }

        public static bool AddCategoryIfNotExists(CategoryListItemViewModel category)
        {
            bool success = false;
            if (!IsCategoryExists(category))
            {
                AddCategory(category);
                success = true;
            }

            return success;
        }

        public static void AddCategory(CategoryListItemViewModel category)
        {
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                SQLiteCommand insertCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = $"INSERT INTO {Category} ({Name}) VALUES ({ParameterName});",
                    Parameters =
                    {
                        new SQLiteParameter(ParameterName, category.Name)
                    }
                };

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
                    CommandText = $"INSERT INTO {Task} " +
                                  $" ({Category}, {Content}, {ListOrder}, " +
                                  $" {IsDone}, {CreationDate}, {ModificationDate}, " +
                                  $" {Color}, {Trashed}, {ReminderId}) " +
                                  $"VALUES ({ParameterCategory}, {ParameterContent}, " +
                                  $" {ParameterListOrder}, {ParameterIsDone}, " +
                                  $" {ParameterCreationDate}, {ParameterModificationDate}, " +
                                  $" {ParameterColor}, {ParameterTrashed}, " +
                                  $" {ParameterReminderId});",
                    Parameters =
                    {
                        new SQLiteParameter(ParameterCategory, taskListItem.Category),
                        new SQLiteParameter(ParameterContent, taskListItem.Content),
                        new SQLiteParameter(ParameterListOrder, taskListItem.ListOrder),
                        new SQLiteParameter(ParameterIsDone, taskListItem.IsDone),
                        new SQLiteParameter(ParameterCreationDate, taskListItem.CreationDate),
                        new SQLiteParameter(ParameterModificationDate, taskListItem.ModificationDate),
                        new SQLiteParameter(ParameterColor, taskListItem.Color),
                        new SQLiteParameter(ParameterTrashed, taskListItem.Trashed),
                        new SQLiteParameter(ParameterReminderId, taskListItem.ReminderId),
                    }
                };

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

        public static bool IsCategoryExists(CategoryListItemViewModel category)
        {
            bool itemExists = false;
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                SQLiteCommand selectCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = $"SELECT * FROM {Category} WHERE {Name} IS {ParameterName}",
                    Parameters =
                    {
                        new SQLiteParameter(ParameterName, category.Name)
                    }
                };

                SQLiteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    itemExists = true;
                    break;
                }

                db.Close();
            }

            return itemExists;
        }

        public static List<CategoryListItemViewModel> GetCategories()
        {
            List<CategoryListItemViewModel> entries = new List<CategoryListItemViewModel>();

            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                SQLiteCommand selectCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = $"SELECT {Name} FROM {Category}"
                };

                SQLiteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(new CategoryListItemViewModel { Name = query.GetString(0) });
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
                    CommandText = $"SELECT * FROM {Reminder}"
                };

                SQLiteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(
                        new ReminderViewModel
                        {
                            Id = query.GetInt32(query.GetOrdinal(Id)),
                            ReminderDate = query.GetInt64(query.GetOrdinal(ReminderDate)),
                            Note = query.GetString(query.GetOrdinal(Note)),
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
                                  $" FROM {Task} " +
                                  $" WHERE {Task}.{Category} IS {ParameterCategory} " +
                                  $" ORDER BY {ListOrder} ;",
                    Parameters = { new SQLiteParameter(ParameterCategory, category) }
                };

                SQLiteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    TaskListItemViewModel readTask = new TaskListItemViewModel
                    {
                        Id = query.GetInt32(query.GetOrdinal(Id)),
                        Category = query.GetString(query.GetOrdinal(Category)),
                        Content = query.GetString(query.GetOrdinal(Content)),
                        ListOrder = query.GetInt32(query.GetOrdinal(ListOrder)),
                        IsDone = Convert.ToBoolean(query.GetInt32(query.GetOrdinal(IsDone))),
                        CreationDate = query.GetInt64(query.GetOrdinal(CreationDate)),
                        ModificationDate = query.GetInt64(query.GetOrdinal(ModificationDate)),
                        Color = query.GetString(query.GetOrdinal(Color)),
                        Trashed = Convert.ToBoolean(query.GetInt32(query.GetOrdinal(Trashed))),
                        ReminderId = query.GetInt32(query.GetOrdinal(ReminderId))
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
                    CommandText = $"DELETE FROM {Task} WHERE {Id} = {ParameterId}",
                    Parameters = { new SQLiteParameter(ParameterId, id) }
                };

                isOperationSuccessful = deleteCommand.ExecuteNonQuery() > 0;

                db.Close();
            }

            return isOperationSuccessful;
        }

        public static bool DeleteCategory(CategoryListItemViewModel category)
        {
            bool isOperationSuccessful;
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={DatabasePath};"))
            {
                db.Open();

                SQLiteCommand deleteCommand = new SQLiteCommand
                {
                    Connection = db,
                    CommandText = $"DELETE FROM {Category} " +
                                  $" WHERE {Name} IS {ParameterName}",
                    Parameters = { new SQLiteParameter(ParameterName, category.Name) }
                };

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
                    CommandText = $"UPDATE {Task} SET " +
                                  $"  {Category} = {ParameterCategory}, " +
                                  $"  {Content} = {ParameterContent}, " +
                                  $"  {ListOrder} = {ParameterListOrder}, " +
                                  $"  {IsDone} = {ParameterIsDone}, " +
                                  $"  {CreationDate} = {ParameterCreationDate}, " +
                                  $"  {ModificationDate} = {ParameterModificationDate}, " +
                                  $"  {Color} = {ParameterColor}, " +
                                  $"  {Trashed} = {ParameterTrashed}, " +
                                  $"  {ReminderId} = {ParameterReminderId} " +
                                  $" WHERE {Id} = {ParameterId};",
                    Parameters =
                    {
                        new SQLiteParameter(ParameterId, task.Id),
                        new SQLiteParameter(ParameterCategory, task.Category),
                        new SQLiteParameter(ParameterContent, task.Content),
                        new SQLiteParameter(ParameterListOrder, task.ListOrder),
                        new SQLiteParameter(ParameterIsDone, task.IsDone),
                        new SQLiteParameter(ParameterCreationDate, task.CreationDate),
                        new SQLiteParameter(ParameterModificationDate, task.ModificationDate),
                        new SQLiteParameter(ParameterColor, task.Color),
                        new SQLiteParameter(ParameterTrashed, task.Trashed),
                        new SQLiteParameter(ParameterReminderId, task.ReminderId)
                    }
                };

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

                // Using transaction to write the database only once
                using (SQLiteTransaction transaction = db.BeginTransaction())
                {
                    foreach (var task in taskList)
                    {
                        SQLiteCommand updateCommand = new SQLiteCommand
                        {
                            CommandText = $"UPDATE {Task} SET " +
                                          $"  {Category} = {ParameterCategory}, " +
                                          $"  {Content} = {ParameterContent}, " +
                                          $"  {ListOrder} = {ParameterListOrder}, " +
                                          $"  {IsDone} = {ParameterIsDone}, " +
                                          $"  {CreationDate} = {ParameterCreationDate}, " +
                                          $"  {ModificationDate} = {ParameterModificationDate}, " +
                                          $"  {Color} = {ParameterColor}, " +
                                          $"  {Trashed} = {ParameterTrashed}, " +
                                          $"  {ReminderId} = {ParameterReminderId} " +
                                          $" WHERE {Id} = {ParameterId};",
                            Parameters =
                            {
                                new SQLiteParameter(ParameterId, task.Id),
                                new SQLiteParameter(ParameterCategory, task.Category),
                                new SQLiteParameter(ParameterContent, task.Content),
                                new SQLiteParameter(ParameterListOrder, task.ListOrder),
                                new SQLiteParameter(ParameterIsDone, task.IsDone),
                                new SQLiteParameter(ParameterCreationDate, task.CreationDate),
                                new SQLiteParameter(ParameterModificationDate, task.ModificationDate),
                                new SQLiteParameter(ParameterColor, task.Color),
                                new SQLiteParameter(ParameterTrashed, task.Trashed),
                                new SQLiteParameter(ParameterReminderId, task.ReminderId)
                            }

                        };

                        modifiedItems += updateCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
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
                    CommandText = $"UPDATE {Category} SET " +
                                  $" {Name} = {ParameterName}" +
                                  $"WHERE {Name} IS {ParameterOldName};",
                    Parameters =
                    {
                        new SQLiteParameter(ParameterOldName, oldName),
                        new SQLiteParameter(ParameterName, newName),
                    }
                };

                // Use parameterized query to prevent SQL injection attacks
                updateCommand.Parameters.AddWithValue(ParameterOldName, oldName);
                updateCommand.Parameters.AddWithValue(ParameterName, newName);

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

                // Using transaction to write the database only once
                using (SQLiteTransaction transaction = db.BeginTransaction())
                {
                    //db.Open();
                    SQLiteCommand updateCommand = db.CreateCommand();

                    foreach (var todoTask in taskList)
                    {
                        updateCommand.CommandText = $"UPDATE {Task} SET " +
                                                    $"  {ListOrder} = {ParameterListOrder} " +
                                                    $"WHERE {Id} = {ParameterId};";

                        // Use parameterized query to prevent SQL injection attacks
                        updateCommand.Parameters.AddWithValue(ParameterId, todoTask.Id);
                        updateCommand.Parameters.AddWithValue(ParameterListOrder, todoTask.ListOrder);

                        modifiedItems += updateCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
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
