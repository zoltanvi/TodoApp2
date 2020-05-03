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
    public sealed class DataAccessLayer : IDisposable
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

        private readonly SQLiteConnection m_Connection;

        public DataAccessLayer()
        {
            m_Connection = new SQLiteConnection($"Data Source={DatabasePath};");
            m_Connection.Open();
        }


        #region Database initializer

        public void InitializeDatabase()
        {
            string prepareCommand = "PRAGMA foreign_keys = ON; ";

            string createCategoryTable =
                $"CREATE TABLE IF NOT EXISTS {Category} ( " +
                $" {Name} TEXT PRIMARY KEY " +
                $"); ";
            string createReminderTable =
                $"CREATE TABLE IF NOT EXISTS {Reminder} ( " +
                $" {Id} INTEGER PRIMARY KEY AUTOINCREMENT, " +
                $" {ReminderDate} INTEGER, " +
                $" {Note} TEXT " +
                $"); ";
            string createTaskTable =
                $"CREATE TABLE IF NOT EXISTS {Task} ( " +
                $" {Id} INTEGER PRIMARY KEY AUTOINCREMENT, " +
                $" {Category} TEXT, " +
                $" {Content} TEXT, " +
                $" {ListOrder} INTEGER, " +
                $" {IsDone} INTEGER, " +
                $" {CreationDate} INTEGER, " +
                $" {ModificationDate} INTEGER, " +
                $" {Color} TEXT, " +
                $" {Trashed} INTEGER, " +
                $" {ReminderId} INTEGER DEFAULT NULL, " +
                $" FOREIGN KEY ({Category}) REFERENCES {Category} ({Name}) ON UPDATE CASCADE ON DELETE CASCADE," +
                $" FOREIGN KEY ({ReminderId}) REFERENCES {Reminder} ({Id}) ON UPDATE CASCADE ON DELETE SET NULL" +
                $"); ";


            if (!File.Exists(DatabasePath))
            {
                FileStream fs = File.Create(DatabasePath);
                fs.Close();
            }

            // Prepare database
            SQLiteCommand dbCommand = new SQLiteCommand(prepareCommand, m_Connection);
            dbCommand.ExecuteReader();

            // Create CATEGORY table
            dbCommand = new SQLiteCommand(createCategoryTable, m_Connection);
            dbCommand.ExecuteReader();

            // Create REMINDER table
            dbCommand = new SQLiteCommand(createReminderTable, m_Connection);
            dbCommand.ExecuteReader();

            // Create TASK table
            dbCommand = new SQLiteCommand(createTaskTable, m_Connection);
            dbCommand.ExecuteReader();

            AddDefaultCategoryIfNotExists();
            //AddDefaultReminderIfNotExists();
        }

        #endregion

        #region Add methods
        public void AddDefaultCategoryIfNotExists()
        {
            if (GetCategories().Count == 0)
            {
                AddCategory(new CategoryListItemViewModel { Name = "Today" });
            }
        }

        public void AddDefaultReminderIfNotExists()
        {
            if (GetReminders().All(x => x.Id != 0))
            {
                AddReminder(0, string.Empty);
            }
        }

        public void AddReminder(long reminderDate, string note)
        {

            SQLiteCommand insertCommand = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"INSERT INTO {Reminder} ({ReminderDate}, {Note}) " +
                              $"VALUES ({ParameterReminderDate}, {ParameterNote});",
                Parameters =
                {
                    new SQLiteParameter(ParameterReminderDate, reminderDate),
                    new SQLiteParameter(ParameterNote, note),
                }
            };

            insertCommand.ExecuteReader();
        }

        public bool AddCategoryIfNotExists(CategoryListItemViewModel category)
        {
            bool success = false;
            if (!IsCategoryExists(category))
            {
                AddCategory(category);
                success = true;
            }

            return success;
        }

        public void AddCategory(CategoryListItemViewModel category)
        {
            SQLiteCommand insertCommand = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"INSERT INTO {Category} ({Name}) VALUES ({ParameterName});",
                Parameters =
                {
                    new SQLiteParameter(ParameterName, category.Name)
                }
            };

            insertCommand.ExecuteReader();
        }

        /// <summary>
        /// Returns the auto generated id of the added task.
        /// </summary>
        /// <param name="taskListItem"></param>
        /// <returns></returns>
        public int AddTask(TaskListItemViewModel taskListItem)
        {
            int taskId = -1;

            SQLiteCommand insertCommand = new SQLiteCommand
            {
                Connection = m_Connection,
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
                Connection = m_Connection,
                CommandText = "SELECT last_insert_rowid();"
            };

            SQLiteDataReader query = selectCommand.ExecuteReader();

            while (query.Read())
            {
                taskId = query.GetInt32(0);
            }

            return taskId;
        }

        #endregion

        #region Get methods

        public bool IsCategoryExists(CategoryListItemViewModel category)
        {
            bool itemExists = false;

            SQLiteCommand selectCommand = new SQLiteCommand
            {
                Connection = m_Connection,
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

            return itemExists;
        }

        public List<CategoryListItemViewModel> GetCategories()
        {
            List<CategoryListItemViewModel> entries = new List<CategoryListItemViewModel>();

            SQLiteCommand selectCommand = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"SELECT {Name} FROM {Category}"
            };

            SQLiteDataReader query = selectCommand.ExecuteReader();

            while (query.Read())
            {
                entries.Add(new CategoryListItemViewModel { Name = query.GetString(0) });
            }

            return entries;
        }

        public List<ReminderViewModel> GetReminders()
        {
            List<ReminderViewModel> entries = new List<ReminderViewModel>();

            SQLiteCommand selectCommand = new SQLiteCommand
            {
                Connection = m_Connection,
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


            return entries;
        }

        public List<TaskListItemViewModel> GetTasks(string category)
        {
            List<TaskListItemViewModel> entries = new List<TaskListItemViewModel>();

            SQLiteCommand selectCommand = new SQLiteCommand
            {
                Connection = m_Connection,
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

            return entries;
        }

        #endregion

        #region Delete methods

        public bool DeleteTask(int id)
        {
            SQLiteCommand deleteCommand = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"DELETE FROM {Task} WHERE {Id} = {ParameterId}",
                Parameters = { new SQLiteParameter(ParameterId, id) }
            };

            return deleteCommand.ExecuteNonQuery() > 0;
        }

        public bool DeleteCategory(CategoryListItemViewModel category)
        {
            SQLiteCommand deleteCommand = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"DELETE FROM {Category} " +
                              $" WHERE {Name} IS {ParameterName}",
                Parameters = { new SQLiteParameter(ParameterName, category.Name) }
            };

            return deleteCommand.ExecuteNonQuery() > 0;
        }

        #endregion

        #region Update methods

        public bool UpdateTask(TaskListItemViewModel task)
        {
            SQLiteCommand updateCommand = new SQLiteCommand
            {
                Connection = m_Connection,
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

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public int UpdateTaskList(IEnumerable<TaskListItemViewModel> taskList)
        {
            int modifiedItems = 0;

            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                foreach (var task in taskList)
                {
                    SQLiteCommand updateCommand = new SQLiteCommand
                    {
                        Connection = m_Connection,
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

            return modifiedItems;
        }

        public bool UpdateCategory(string oldName, string newName)
        {
            SQLiteCommand updateCommand = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"UPDATE {Category} SET " +
                              $" {Name} = {ParameterName}" +
                              $"WHERE {Name} IS {ParameterOldName};",
                Parameters =
                {
                        new SQLiteParameter(ParameterOldName, oldName),
                        new SQLiteParameter(ParameterName, newName)
                }
            };

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public int UpdateTaskListOrders(IEnumerable<TaskListItemViewModel> taskList)
        {
            int modifiedItems = 0;

            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                foreach (var todoTask in taskList)
                {
                    SQLiteCommand updateCommand = new SQLiteCommand
                    {
                        Connection = m_Connection,
                        CommandText = $"UPDATE {Task} SET " +
                                      $"  {ListOrder} = {ParameterListOrder} " +
                                      $"WHERE {Id} = {ParameterId};",
                        Parameters =
                        {
                            new SQLiteParameter(ParameterId, todoTask.Id),
                            new SQLiteParameter(ParameterListOrder, todoTask.ListOrder),
                        }
                    };

                    modifiedItems += updateCommand.ExecuteNonQuery();
                }

                transaction.Commit();
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

        public void Dispose()
        {
            m_Connection.Close();
            m_Connection.Dispose();
        }
    }
}
