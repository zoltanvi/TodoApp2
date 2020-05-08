using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.SQLite;
using TodoApp2.Core.Helpers;

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
        private const string CategoryId = "CategoryId";


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
        private const string ParameterCategoryId = "@" + CategoryId;
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
                $" {Id} INTEGER PRIMARY KEY AUTOINCREMENT, " +
                $" {Name} TEXT, " +
                $" {ListOrder} INTEGER, " +
                $" {Trashed} INTEGER " +
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
                $" {CategoryId} INTEGER, " +
                $" {Content} TEXT, " +
                $" {ListOrder} INTEGER, " +
                $" {IsDone} INTEGER, " +
                $" {CreationDate} INTEGER, " +
                $" {ModificationDate} INTEGER, " +
                $" {Color} TEXT, " +
                $" {Trashed} INTEGER, " +
                $" {ReminderId} INTEGER DEFAULT NULL, " +
                $" FOREIGN KEY ({CategoryId}) REFERENCES {Category} ({Id}) ON UPDATE CASCADE ON DELETE CASCADE," +
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
        }

        #endregion

        #region Add methods
        public void AddDefaultCategoryIfNotExists()
        {
            if (GetCategories().Count == 0)
            {
                // This should only happen when the application database is just created
                AddCategory(new CategoryListItemViewModel { Name = "Today" });
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
                // Add category to database, get back the generated ID
                int categoryId = AddCategory(category);
               
                // Set the generated ID to the category in the memory
                category.Id = categoryId;
                
                success = true;
            }

            return success;
        }

        public int AddCategory(CategoryListItemViewModel category)
        {
            SQLiteCommand insertCommand = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"INSERT INTO {Category} ({Name}, {ListOrder}, {Trashed}) " +
                              $" VALUES ({ParameterName}, {ParameterListOrder}, {ParameterTrashed});",
                Parameters =
                {
                    new SQLiteParameter(ParameterName, category.Name),
                    new SQLiteParameter(ParameterListOrder, category.ListOrder),
                    new SQLiteParameter(ParameterTrashed, category.Trashed)
                }
            };

            insertCommand.ExecuteReader();
            
            int categoryId = GetLastInsertedId();

            return categoryId;
        }

        /// <summary>
        /// Returns the auto generated id of the added task.
        /// </summary>
        /// <param name="taskListItem"></param>
        /// <returns></returns>
        public int AddTask(TaskListItemViewModel taskListItem)
        {
            SQLiteCommand insertCommand = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"INSERT INTO {Task} " +
                              $" ({CategoryId}, {Content}, {ListOrder}, {IsDone}, {CreationDate}, " +
                              $" {ModificationDate}, {Color}, {Trashed}, {ReminderId}) " +
                              $" VALUES ({ParameterCategoryId}, {ParameterContent}, {ParameterListOrder}, {ParameterIsDone}, " +
                              $" {ParameterCreationDate}, {ParameterModificationDate}, {ParameterColor}, {ParameterTrashed}, " +
                              $" {ParameterReminderId});",
                Parameters =
                {
                    new SQLiteParameter(ParameterCategoryId, taskListItem.CategoryId),
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

            int taskId = GetLastInsertedId();

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
                Parameters = { new SQLiteParameter(ParameterName, category.Name) }
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
                CommandText = $"SELECT * FROM {Category} ORDER BY {ListOrder}"
            };

            SQLiteDataReader query = selectCommand.ExecuteReader();

            while (query.Read())
            {
                entries.Add(new CategoryListItemViewModel
                {
                    Id = query.SafeGetInt(Id),
                    Name = query.SafeGetString(Name),
                    ListOrder = query.SafeGetInt(ListOrder),
                    Trashed = query.SafeGetBoolFromInt(Trashed)
                });
            }

            return entries;
        }

        public CategoryListItemViewModel GetCategory(int id)
        {
            CategoryListItemViewModel entry = null;

            SQLiteCommand selectCommand = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"SELECT * FROM {Category} WHERE {Id} = {ParameterId}",
                Parameters = { new SQLiteParameter(ParameterId, id)}
            };

            SQLiteDataReader query = selectCommand.ExecuteReader();

            while (query.Read())
            {
                entry = new CategoryListItemViewModel
                {
                    Id = query.SafeGetInt(Id),
                    Name = query.SafeGetString(Name),
                    ListOrder = query.SafeGetInt(ListOrder),
                    Trashed = query.SafeGetBoolFromInt(Trashed)
                };
            }

            return entry;
        }

        public CategoryListItemViewModel GetCategory(string name)
        {
            CategoryListItemViewModel entry = null;

            SQLiteCommand selectCommand = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"SELECT * FROM {Category} WHERE {Name} = {ParameterName}",
                Parameters = { new SQLiteParameter(ParameterName, name) }
            };

            SQLiteDataReader query = selectCommand.ExecuteReader();

            while (query.Read())
            {
                entry = new CategoryListItemViewModel
                {
                    Id = query.SafeGetInt(Id),
                    Name = query.SafeGetString(Name),
                    ListOrder = query.SafeGetInt(ListOrder),
                    Trashed = query.SafeGetBoolFromInt(Trashed)
                };
            }

            return entry;
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
                entries.Add(new ReminderViewModel
                {
                    Id = query.SafeGetInt(Id),
                    ReminderDate = query.SafeGetLong(ReminderDate),
                    Note = query.SafeGetString(Note),
                });
            }

            return entries;
        }

        public List<TaskListItemViewModel> GetTasks(int categoryId)
        {
            List<TaskListItemViewModel> entries = new List<TaskListItemViewModel>();

            SQLiteCommand selectCommand = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"SELECT * " +
                                  $" FROM {Task} " +
                                  $" WHERE {CategoryId} = {ParameterCategoryId} " +
                                  $" ORDER BY {ListOrder} ;",
                Parameters = { new SQLiteParameter(ParameterCategoryId, categoryId) }
            };

            SQLiteDataReader query = selectCommand.ExecuteReader();

            while (query.Read())
            {
                TaskListItemViewModel readTask = new TaskListItemViewModel
                {
                    Id = query.SafeGetInt(Id),
                    CategoryId = query.SafeGetInt(CategoryId),
                    Content = query.SafeGetString(Content),
                    ListOrder = query.SafeGetInt(ListOrder),
                    IsDone = query.SafeGetBoolFromInt(IsDone),
                    CreationDate = query.SafeGetLong(CreationDate),
                    ModificationDate = query.SafeGetLong(ModificationDate),
                    Color = query.SafeGetString(Color),
                    Trashed = query.SafeGetBoolFromInt(Trashed),
                    ReminderId = query.SafeGetNullableInt(ReminderId)
                };

                entries.Add(readTask);
            }

            return entries;
        }

        #endregion

        #region Delete methods

        //public bool DeleteCategory(CategoryListItemViewModel category)
        //{
        //    SQLiteCommand deleteCommand = new SQLiteCommand
        //    {
        //        Connection = m_Connection,
        //        CommandText = $"DELETE FROM {Category} " +
        //                      $" WHERE {Id} IS {ParameterId}",
        //        Parameters = { new SQLiteParameter(Id, category.Id) }
        //    };

        //    return deleteCommand.ExecuteNonQuery() > 0;
        //}

        #endregion

        #region Update methods

        public bool UpdateTask(TaskListItemViewModel task)
        {
            SQLiteCommand updateCommand = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"UPDATE {Task} SET " +
                              $"  {CategoryId} = {ParameterCategoryId}, " +
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
                    new SQLiteParameter(ParameterCategoryId, task.CategoryId),
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
                                      $"  {CategoryId} = {ParameterCategoryId}, " +
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
                            new SQLiteParameter(ParameterCategoryId, task.CategoryId),
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
                                      $" {ListOrder} = {ParameterListOrder} " +
                                      $" WHERE {Id} = {ParameterId};",
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

        public bool UpdateCategory(CategoryListItemViewModel category)
        {
            SQLiteCommand updateCommand = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"UPDATE {Category} SET " +
                              $" {Name} = {ParameterName}, " +
                              $" {ListOrder} = {ParameterListOrder}, " +
                              $" {Trashed} = {ParameterTrashed} " +
                              $" WHERE {Id} IS {ParameterId};",
                Parameters =
                {
                    new SQLiteParameter(ParameterName, category.Name),
                    new SQLiteParameter(ParameterListOrder, category.ListOrder),
                    new SQLiteParameter(ParameterTrashed, category.Trashed),
                    new SQLiteParameter(ParameterId, category.Id)
                }
            };

            return updateCommand.ExecuteNonQuery() > 0;
        }

        public int UpdateCategoryListOrders(IEnumerable<CategoryListItemViewModel> categoryList)
        {
            int modifiedItems = 0;

            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                foreach (var category in categoryList)
                {
                    SQLiteCommand updateCommand = new SQLiteCommand
                    {
                        Connection = m_Connection,
                        CommandText = $"UPDATE {Category} SET " +
                                      $" {ListOrder} = {ParameterListOrder} " +
                                      $" WHERE {Id} = {ParameterId};",
                        Parameters =
                        {
                            new SQLiteParameter(ParameterId, category.Id),
                            new SQLiteParameter(ParameterListOrder, category.ListOrder),
                        }
                    };

                    modifiedItems += updateCommand.ExecuteNonQuery();
                }

                transaction.Commit();
            }

            return modifiedItems;
        }

        #endregion

        #region Helpers

        private int GetLastInsertedId()
        {
            int lastInsertedId = -1;
            SQLiteCommand selectCommand = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = "SELECT last_insert_rowid();"
            };

            SQLiteDataReader query = selectCommand.ExecuteReader();

            while (query.Read())
            {
                lastInsertedId = query.GetInt32(0);
            }

            return lastInsertedId;
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
