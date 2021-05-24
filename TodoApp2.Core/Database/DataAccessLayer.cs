using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace TodoApp2.Core
{
    /// <summary>
    /// The data access layer to access information from the database
    /// </summary>
    public sealed class DataAccessLayer : IDisposable
    {
        public const long DefaultListOrder = long.MaxValue / 2;
        public const long ListOrderInterval = 1_000_000_000_000;
        
        public const string OfflineDatabaseName = "TodoApp2Database.db";
        public const string OnlineDatabaseName = "TodoApp2Database-online.db";
        public static string OfflineDatabasePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), OfflineDatabaseName);
        public static string OnlineDatabasePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), OnlineDatabaseName);

        #region Private Constants

        private const string Task = "Task";
        private const string Settings = "Settings";

        private const string Name = "Name";
        private const string Id = "Id";
        private const string Key = "Key";
        private const string Value = "Value";
        private const string ReminderDate = "ReminderDate";
        private const string IsReminderOn = "IsReminderOn";
        private const string Category = "Category";
        private const string Content = "Content";
        private const string ListOrder = "ListOrder";
        private const string IsDone = "IsDone";
        private const string CreationDate = "CreationDate";
        private const string ModificationDate = "ModificationDate";
        private const string Color = "Color";
        private const string Trashed = "Trashed";
        private const string CategoryId = "CategoryId";

        private const string ParameterName = "@" + Name;
        private const string ParameterId = "@" + Id;
        private const string ParameterKey = "@" + Key;
        private const string ParameterValue = "@" + Value;
        private const string ParameterReminderDate = "@" + ReminderDate;
        private const string ParameterIsReminderOn = "@" + IsReminderOn;
        private const string ParameterCategoryId = "@" + CategoryId;
        private const string ParameterContent = "@" + Content;
        private const string ParameterListOrder = "@" + ListOrder;
        private const string ParameterIsDone = "@" + IsDone;
        private const string ParameterCreationDate = "@" + CreationDate;
        private const string ParameterModificationDate = "@" + ModificationDate;
        private const string ParameterColor = "@" + Color;
        private const string ParameterTrashed = "@" + Trashed;

        #endregion Private Constants

        private readonly SQLiteConnection m_Connection;

        public DataAccessLayer(bool online = false)
        {
            string dbPath = online ? OnlineDatabasePath : OfflineDatabasePath;
            m_Connection = new SQLiteConnection($"Data Source={dbPath};");
            m_Connection.Open();
        }

        public void InitializeDatabase()
        {
            string prepareCommand = "PRAGMA foreign_keys = ON; ";

            string createSettingsTable =
                $"CREATE TABLE IF NOT EXISTS {Settings} ( " +
                $" {Id} INTEGER PRIMARY KEY, " +
                $" {Key} TEXT, " +
                $" {Value} TEXT " +
                $"); ";
            string createCategoryTable =
                $"CREATE TABLE IF NOT EXISTS {Category} ( " +
                $" {Id} INTEGER PRIMARY KEY, " +
                $" {Name} TEXT, " +
                $" {ListOrder} TEXT DEFAULT ('{DefaultListOrder}'), " +
                $" {Trashed} INTEGER " +
                $"); ";
            string createTaskTable =
                $"CREATE TABLE IF NOT EXISTS {Task} ( " +
                $" {Id} INTEGER PRIMARY KEY, " +
                $" {CategoryId} INTEGER, " +
                $" {Content} TEXT, " +
                $" {ListOrder} TEXT DEFAULT ('{DefaultListOrder}'), " +
                $" {IsDone} INTEGER DEFAULT (0), " +
                $" {CreationDate} INTEGER, " +
                $" {ModificationDate} INTEGER, " +
                $" {Color} TEXT, " +
                $" {Trashed} INTEGER DEFAULT (0), " +
                $" {ReminderDate} INTEGER DEFAULT (0), " +
                $" {IsReminderOn} INTEGER DEFAULT (0), " +
                $" FOREIGN KEY ({CategoryId}) REFERENCES {Category} ({Id}) ON UPDATE CASCADE ON DELETE CASCADE " +
                $"); ";

            if (!File.Exists(OfflineDatabasePath))
            {
                FileStream fs = File.Create(OfflineDatabasePath);
                fs.Close();
            }

            // Prepare database
            SQLiteCommand dbCommand = new SQLiteCommand(prepareCommand, m_Connection);
            dbCommand.ExecuteReader();
            dbCommand.Dispose();

            // Create SETTINGS table
            dbCommand = new SQLiteCommand(createSettingsTable, m_Connection);
            dbCommand.ExecuteReader();
            dbCommand.Dispose();

            // Create CATEGORY table
            dbCommand = new SQLiteCommand(createCategoryTable, m_Connection);
            dbCommand.ExecuteReader();
            dbCommand.Dispose();

            // Create TASK table
            dbCommand = new SQLiteCommand(createTaskTable, m_Connection);
            dbCommand.ExecuteReader();
            dbCommand.Dispose();

            AddDefaultCategoryIfNotExists();
            AddDefaultSettingsIfNotExists();
        }

        #region Settings

        /// <summary>
        /// Gets every record from the Settings table
        /// </summary>
        /// <returns></returns>
        public List<SettingsModel> GetSettings()
        {
            List<SettingsModel> items = new List<SettingsModel>();

            SQLiteCommand command = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"SELECT * FROM {Settings}"
            };

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new SettingsModel
                {
                    Id = reader.SafeGetInt(Id),
                    Key = reader.SafeGetString(Key),
                    Value = reader.SafeGetString(Value)
                });
            }

            command.Dispose();
            reader.Dispose();

            return items;
        }

        /// <summary>
        /// Gets the next available Id for a Settings record
        /// </summary>
        /// <returns></returns>
        public int GetSettingsNextId()
        {
            int nextId = int.MinValue;

            SQLiteCommand command = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"SELECT * FROM {Settings} ORDER BY {Id} DESC LIMIT 1"
            };

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                nextId = reader.SafeGetInt(Id) + 1;
                break;
            }

            command.Dispose();
            reader.Dispose();

            return nextId;
        }

        /// <summary>
        /// Inserts a new record into the Settings table
        /// </summary>
        /// <param name="settings"></param>
        public void AddSetting(SettingsModel settings)
        {
            SQLiteCommand command = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"INSERT INTO {Settings} ({Id}, {Key}, {Value}) " +
                              $" VALUES ({ParameterId}, {ParameterKey}, {ParameterValue});",
                Parameters =
                {
                    new SQLiteParameter(ParameterId, settings.Id),
                    new SQLiteParameter(ParameterKey, settings.Key),
                    new SQLiteParameter(ParameterValue, settings.Value)
                }
            };

            command.ExecuteReader();
            command.Dispose();
        }

        /// <summary>
        /// Inserts each setting item from the list as a new record into the Settings table
        /// </summary>
        /// <param name="settingsList"></param>
        public void AddSettings(IEnumerable<SettingsModel> settingsList)
        {
            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                foreach (var settings in settingsList)
                {
                    SQLiteCommand command = new SQLiteCommand
                    {
                        Connection = m_Connection,
                        CommandText = $"INSERT INTO {Settings} ({Id}, {Key}, {Value}) " +
                                      $" VALUES ({ParameterId}, {ParameterKey}, {ParameterValue});",
                        Parameters =
                        {
                            new SQLiteParameter(ParameterId, settings.Id),
                            new SQLiteParameter(ParameterKey, settings.Key),
                            new SQLiteParameter(ParameterValue, settings.Value)
                        }
                    };

                    command.ExecuteNonQuery();
                    command.Dispose();
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// Updates a record in the Settings table
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public bool UpdateSetting(SettingsModel settings)
        {
            SQLiteCommand command = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"UPDATE {Settings} SET " +
                              $"  {Key} = {ParameterKey}, " +
                              $"  {Value} = {ParameterValue} " +
                              $" WHERE {Id} = {ParameterId};",
                Parameters =
                {
                    new SQLiteParameter(ParameterId, settings.Id),
                    new SQLiteParameter(ParameterKey, settings.Key),
                    new SQLiteParameter(ParameterValue, settings.Value),
                }
            };

            var result = command.ExecuteNonQuery() > 0;
            command.Dispose();
            return result;
        }

        /// <summary>
        /// Updates each record in the Settings table from the provided list
        /// </summary>
        /// <param name="settingsList"></param>
        /// <returns></returns>
        public int UpdateSettings(IEnumerable<SettingsModel> settingsList)
        {
            int modifiedItems = 0;

            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                foreach (var setting in settingsList)
                {
                    SQLiteCommand command = new SQLiteCommand
                    {
                        Connection = m_Connection,
                        CommandText = $"UPDATE {Settings} SET " +
                                      $"  {Value} = {ParameterValue} " +
                                      $" WHERE {Key} = {ParameterKey};",
                        Parameters =
                        {
                            new SQLiteParameter(ParameterKey, setting.Key),
                            new SQLiteParameter(ParameterValue, setting.Value),
                        }
                    };

                    modifiedItems += command.ExecuteNonQuery();
                    command.Dispose();
                }

                transaction.Commit();
            }

            return modifiedItems;
        }

        /// <summary>
        /// Deletes a record from the Settings table
        /// </summary>
        /// <param name="settings"></param>
        public bool DeleteSettings(SettingsModel settings)
        {
            SQLiteCommand command = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"DELETE FROM {Settings} " +
                              $" WHERE {Id} = {ParameterId};",
                Parameters =
                {
                    new SQLiteParameter(ParameterId, settings.Id),
                }
            };

            var result = command.ExecuteNonQuery() > 0;
            command.Dispose();
            return result;
        }

        #endregion Settings

        #region Category

        /// <summary>
        /// Gets every record from the Category table
        /// </summary>
        /// <returns></returns>
        public List<CategoryListItemViewModel> GetCategories()
        {
            List<CategoryListItemViewModel> items = new List<CategoryListItemViewModel>();

            SQLiteCommand command = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"SELECT * FROM {Category} ORDER BY {ListOrder}"
            };

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new CategoryListItemViewModel
                {
                    Id = reader.SafeGetInt(Id),
                    Name = reader.SafeGetString(Name),
                    ListOrder = reader.SafeGetLongFromString(ListOrder),
                    Trashed = reader.SafeGetBoolFromInt(Trashed)
                });
            }

            command.Dispose();
            reader.Dispose();
            return items;
        }

        /// <summary>
        /// Gets the category record with the provided name from the Category table
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CategoryListItemViewModel GetCategory(string name)
        {
            CategoryListItemViewModel item = null;

            SQLiteCommand command = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"SELECT * FROM {Category} WHERE {Name} = {ParameterName}",
                Parameters = { new SQLiteParameter(ParameterName, name) }
            };

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                item = new CategoryListItemViewModel
                {
                    Id = reader.SafeGetInt(Id),
                    Name = reader.SafeGetString(Name),
                    ListOrder = reader.SafeGetLongFromString(ListOrder),
                    Trashed = reader.SafeGetBoolFromInt(Trashed)
                };
            }

            command.Dispose();
            reader.Dispose();
            return item;
        }

        /// <summary>
        /// Gets the next available Id for a Category record
        /// </summary>
        /// <returns></returns>
        public int GetCategoryNextId()
        {
            int nextId = int.MinValue;

            SQLiteCommand command = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"SELECT * FROM {Category} ORDER BY {Id} DESC LIMIT 1"
            };

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                nextId = reader.SafeGetInt(Id) + 1;
                break;
            }

            command.Dispose();
            reader.Dispose();
            return nextId;
        }

        /// <summary>
        /// Gets the first ListOrder for a Category record
        /// </summary>
        /// <returns></returns>
        public long GetCategoryFirstListOrder()
        {
            return GetListOrder(Category, true);
        }

        /// <summary>
        /// Gets the last ListOrder for a Category record
        /// </summary>
        /// <returns></returns>
        public long GetCategoryLastListOrder()
        {
            return GetListOrder(Category, false);
        }

        /// <summary>
        /// Inserts a new record into the Category table
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public void AddCategory(CategoryListItemViewModel category)
        {
            SQLiteCommand command = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"INSERT INTO {Category} ({Id}, {Name}, {ListOrder}, {Trashed}) " +
                              $" VALUES ({ParameterId}, {ParameterName}, {ParameterListOrder}, {ParameterTrashed});",
                Parameters =
                {
                    new SQLiteParameter(ParameterId, category.Id),
                    new SQLiteParameter(ParameterName, category.Name),
                    new SQLiteParameter(ParameterListOrder, category.ListOrder.ToString("D19")),
                    new SQLiteParameter(ParameterTrashed, category.Trashed)
                }
            };

            command.ExecuteReader();
            command.Dispose();
        }

        /// <summary>
        /// Updates a record in the Category table
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public bool UpdateCategory(CategoryListItemViewModel category)
        {
            SQLiteCommand command = new SQLiteCommand
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
                    new SQLiteParameter(ParameterListOrder, category.ListOrder.ToString("D19")),
                    new SQLiteParameter(ParameterTrashed, category.Trashed),
                    new SQLiteParameter(ParameterId, category.Id)
                }
            };

            var result = command.ExecuteNonQuery() > 0;
            command.Dispose();
            return result;
        }

        /// <summary>
        /// Updates the ListOrder for each record in the Category table
        /// </summary>
        /// <param name="categoryList"></param>
        /// <returns></returns>
        public int UpdateCategoryListOrders(IEnumerable<CategoryListItemViewModel> categoryList)
        {
            int modifiedItems = 0;

            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                foreach (var category in categoryList)
                {
                    SQLiteCommand command = new SQLiteCommand
                    {
                        Connection = m_Connection,
                        CommandText = $"UPDATE {Category} SET " +
                                      $" {ListOrder} = {ParameterListOrder} " +
                                      $" WHERE {Id} = {ParameterId};",
                        Parameters =
                        {
                            new SQLiteParameter(ParameterId, category.Id),
                            new SQLiteParameter(ParameterListOrder, category.ListOrder.ToString("D19")),
                        }
                    };

                    modifiedItems += command.ExecuteNonQuery();
                    command.Dispose();
                }

                transaction.Commit();
            }

            return modifiedItems;
        }

        #endregion Category

        #region Task

        /// <summary>
        /// Gets every record from the Task table
        /// </summary>
        /// <returns></returns>
        public List<TaskListItemViewModel> GetTasks()
        {
            List<TaskListItemViewModel> items = new List<TaskListItemViewModel>();

            SQLiteCommand command = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"SELECT * FROM {Task} ORDER BY {ListOrder} ;",
            };

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                TaskListItemViewModel readTask = new TaskListItemViewModel
                {
                    Id = reader.SafeGetInt(Id),
                    CategoryId = reader.SafeGetInt(CategoryId),
                    Content = reader.SafeGetString(Content),
                    ListOrder = reader.SafeGetLongFromString(ListOrder),
                    IsDone = reader.SafeGetBoolFromInt(IsDone),
                    CreationDate = reader.SafeGetLong(CreationDate),
                    ModificationDate = reader.SafeGetLong(ModificationDate),
                    Color = reader.SafeGetString(Color),
                    Trashed = reader.SafeGetBoolFromInt(Trashed),
                    ReminderDate = reader.SafeGetLong(ReminderDate),
                    IsReminderOn = reader.SafeGetBoolFromInt(IsReminderOn)
                };

                items.Add(readTask);
            }
            
            command.Dispose();
            reader.Dispose();
            return items;
        }

        /// <summary>
        /// Gets the task with the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the task to query.</param>
        /// <returns></returns>
        public TaskListItemViewModel GetTask(int id)
        {
            TaskListItemViewModel readTask = null;
            SQLiteCommand command = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"SELECT * FROM {Task} WHERE {Id} = {ParameterId} ;",
                Parameters =
                {
                    new SQLiteParameter(ParameterId, id), 
                }
            };

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                readTask = new TaskListItemViewModel
                {
                    Id = reader.SafeGetInt(Id),
                    CategoryId = reader.SafeGetInt(CategoryId),
                    Content = reader.SafeGetString(Content),
                    ListOrder = reader.SafeGetLongFromString(ListOrder),
                    IsDone = reader.SafeGetBoolFromInt(IsDone),
                    CreationDate = reader.SafeGetLong(CreationDate),
                    ModificationDate = reader.SafeGetLong(ModificationDate),
                    Color = reader.SafeGetString(Color),
                    Trashed = reader.SafeGetBoolFromInt(Trashed),
                    ReminderDate = reader.SafeGetLong(ReminderDate),
                    IsReminderOn = reader.SafeGetBoolFromInt(IsReminderOn),
                };

                break;
            }

            command.Dispose();
            reader.Dispose();
            return readTask;
        }

        /// <summary>
        /// Gets the next available Id for a Task record
        /// </summary>
        /// <returns></returns>
        public int GetTaskNextId()
        {
            int nextId = 0;

            SQLiteCommand command = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"SELECT * FROM {Task} ORDER BY {Id} DESC LIMIT 1"
            };

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                nextId = reader.SafeGetInt(Id) + 1;
                break;
            }

            command.Dispose();
            reader.Dispose();
            return nextId;
        }

        /// <summary>
        /// Gets the first ListOrder for a Task record
        /// </summary>
        /// <returns></returns>
        public long GetTaskFirstListOrder()
        {
            return GetListOrder(Task, true);
        }

        /// <summary>
        /// Gets the last ListOrder for a Task record
        /// </summary>
        /// <returns></returns>
        public long GetTaskLastListOrder()
        {
            return GetListOrder(Task, false);
        }

        /// <summary>
        /// Inserts a new record into the Task table
        /// </summary>
        /// <param name="taskListItem"></param>
        /// <returns></returns>
        public void AddTask(TaskListItemViewModel taskListItem)
        {
            SQLiteCommand command = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"INSERT INTO {Task} " +
                              $" ({Id}, {CategoryId}, {Content}, {ListOrder}, {IsDone}, {CreationDate}, {ModificationDate}, {Color}, {Trashed}, {ReminderDate}, {IsReminderOn}) " +
                              $" VALUES ({ParameterId}, {ParameterCategoryId}, {ParameterContent}, {ParameterListOrder}, {ParameterIsDone}, " +
                              $" {ParameterCreationDate}, {ParameterModificationDate}, {ParameterColor}, {ParameterTrashed}, {ParameterReminderDate}, {ParameterIsReminderOn});",
                Parameters =
                {
                    new SQLiteParameter(ParameterId, taskListItem.Id),
                    new SQLiteParameter(ParameterCategoryId, taskListItem.CategoryId),
                    new SQLiteParameter(ParameterContent, taskListItem.Content),
                    new SQLiteParameter(ParameterListOrder, taskListItem.ListOrder.ToString("D19")),
                    new SQLiteParameter(ParameterIsDone, taskListItem.IsDone),
                    new SQLiteParameter(ParameterCreationDate, taskListItem.CreationDate),
                    new SQLiteParameter(ParameterModificationDate, taskListItem.ModificationDate),
                    new SQLiteParameter(ParameterColor, taskListItem.Color),
                    new SQLiteParameter(ParameterTrashed, taskListItem.Trashed),
                    new SQLiteParameter(ParameterReminderDate, taskListItem.ReminderDate),
                    new SQLiteParameter(ParameterIsReminderOn, taskListItem.IsReminderOn),
                }
            };

            command.ExecuteReader();
            command.Dispose();
        }

        /// <summary>
        /// Updates a record in the Task table
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool UpdateTask(TaskListItemViewModel task)
        {
            SQLiteCommand command = new SQLiteCommand
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
                              $"  {ReminderDate} = {ParameterReminderDate}, " +
                              $"  {IsReminderOn} = {ParameterIsReminderOn} " +
                              $" WHERE {Id} = {ParameterId};",
                Parameters =
                {
                    new SQLiteParameter(ParameterId, task.Id),
                    new SQLiteParameter(ParameterCategoryId, task.CategoryId),
                    new SQLiteParameter(ParameterContent, task.Content),
                    new SQLiteParameter(ParameterListOrder, task.ListOrder.ToString("D19")),
                    new SQLiteParameter(ParameterIsDone, task.IsDone),
                    new SQLiteParameter(ParameterCreationDate, task.CreationDate),
                    new SQLiteParameter(ParameterModificationDate, task.ModificationDate),
                    new SQLiteParameter(ParameterColor, task.Color),
                    new SQLiteParameter(ParameterTrashed, task.Trashed),
                    new SQLiteParameter(ParameterReminderDate, task.ReminderDate),
                    new SQLiteParameter(ParameterIsReminderOn, task.IsReminderOn),
                }
            };
            var result = command.ExecuteNonQuery() > 0;
            command.Dispose();
            return result;
        }

        /// <summary>
        /// Updates each record in the Task table from the provided list
        /// </summary>
        /// <param name="taskList"></param>
        /// <returns></returns>
        public int UpdateTaskList(IEnumerable<TaskListItemViewModel> taskList)
        {
            int modifiedItems = 0;

            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                foreach (var task in taskList)
                {
                    SQLiteCommand command = new SQLiteCommand
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
                                      $"  {ReminderDate} = {ParameterReminderDate}, " +
                                      $"  {IsReminderOn} = {ParameterIsReminderOn} " +
                                      $" WHERE {Id} = {ParameterId};",
                        Parameters =
                        {
                            new SQLiteParameter(ParameterId, task.Id),
                            new SQLiteParameter(ParameterCategoryId, task.CategoryId),
                            new SQLiteParameter(ParameterContent, task.Content),
                            new SQLiteParameter(ParameterListOrder, task.ListOrder.ToString("D19")),
                            new SQLiteParameter(ParameterIsDone, task.IsDone),
                            new SQLiteParameter(ParameterCreationDate, task.CreationDate),
                            new SQLiteParameter(ParameterModificationDate, task.ModificationDate),
                            new SQLiteParameter(ParameterColor, task.Color),
                            new SQLiteParameter(ParameterTrashed, task.Trashed),
                            new SQLiteParameter(ParameterReminderDate, task.ReminderDate),
                            new SQLiteParameter(ParameterIsReminderOn, task.IsReminderOn),
                        }
                    };

                    modifiedItems += command.ExecuteNonQuery();
                    command.Dispose();
                }

                transaction.Commit();
            }

            return modifiedItems;
        }

        /// <summary>
        /// Updates the ListOrder for each record in the Task table
        /// </summary>
        /// <param name="taskList"></param>
        /// <returns></returns>
        public int UpdateTaskListOrders(IEnumerable<TaskListItemViewModel> taskList)
        {
            int modifiedItems = 0;

            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                foreach (var todoTask in taskList)
                {
                    SQLiteCommand command = new SQLiteCommand
                    {
                        Connection = m_Connection,
                        CommandText = $"UPDATE {Task} SET " +
                                      $" {ListOrder} = {ParameterListOrder} " +
                                      $" WHERE {Id} = {ParameterId};",
                        Parameters =
                        {
                            new SQLiteParameter(ParameterId, todoTask.Id),
                            new SQLiteParameter(ParameterListOrder, todoTask.ListOrder.ToString("D19")),
                        }
                    };

                    modifiedItems += command.ExecuteNonQuery();
                    command.Dispose();
                }

                transaction.Commit();
            }

            return modifiedItems;
        }

        #endregion Task

        private void AddDefaultCategoryIfNotExists()
        {
            if (GetCategories().Count == 0)
            {
                // This should only happen when the application database is just created
                AddCategory(new CategoryListItemViewModel
                {
                    Name = "Today",
                    ListOrder = DefaultListOrder
                });
            }
        }

        private void AddDefaultSettingsIfNotExists()
        {
            const string windowLeftPos = "WindowLeftPos";
            const string windowTopPos = "WindowTopPos";
            const string windowWidth = "WindowWidth";
            const string windowHeight = "WindowHeight";
            const string currentCategory = "CurrentCategory";

            List<string> keys = new List<string> { windowLeftPos, windowTopPos, windowWidth, windowHeight };

            List<SettingsModel> defaultSettings = new List<SettingsModel>
            {
               new SettingsModel {Id = 0, Key = windowLeftPos, Value = "100"},
                new SettingsModel {Id = 1, Key = windowTopPos, Value = "100"},
                new SettingsModel {Id = 2, Key = windowWidth, Value = "380"},
                new SettingsModel {Id = 3, Key = windowHeight, Value = "500"},
                new SettingsModel {Id = 4, Key = currentCategory, Value = "Today"}
            };

            var settings = GetSettings();
            // If none of the default settings are in the database, insert them
            if (!settings.Select(s => s.Key).Any(k => keys.Any(x => x == k)))
            {
                AddSettings(defaultSettings);
            }
        }

        /// <summary>
        /// Gets the first or last ListOrder for a <see cref="table"/> record.
        /// </summary>
        /// <param name="table">The database table to query.</param>
        /// <param name="first">If true, queries the first ListOrder, otherwise queries the last ListOrder.</param>
        /// <returns>Returns the query result.</returns>
        private long GetListOrder(string table, bool first)
        {
            string ordering = first ? string.Empty : "DESC";
            SQLiteCommand command = new SQLiteCommand
            {
                Connection = m_Connection,
                CommandText = $"SELECT * FROM {table} WHERE {Trashed} = 0 ORDER BY {ListOrder} {ordering} LIMIT 1"
            };

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                return reader.SafeGetLongFromString(ListOrder);
            }

            command.Dispose();
            reader.Dispose();
            return DefaultListOrder;
        }

        public void Dispose()
        {
            m_Connection?.Close();
            m_Connection?.Dispose();
        }
    }
}