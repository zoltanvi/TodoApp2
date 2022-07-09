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
        #region Nested classes

        private static class Table
        {
            public const string Task = "Task";
            public const string Settings = "Settings";
        }

        private static class Column
        {
            public const string Name = "Name";
            public const string Id = "Id";
            public const string Key = "Key";
            public const string Value = "Value";
            public const string ReminderDate = "ReminderDate";
            public const string IsReminderOn = "IsReminderOn";
            public const string Category = "Category";
            public const string Content = "Content";
            public const string ListOrder = "ListOrder";
            public const string Pinned = "Pinned";
            public const string IsDone = "IsDone";
            public const string CreationDate = "CreationDate";
            public const string ModificationDate = "ModificationDate";
            public const string Color = "Color";
            public const string Trashed = "Trashed";
            public const string CategoryId = "CategoryId";
        }

        private static class Parameter
        {
            public const string Name = "@" + Column.Name;
            public const string Id = "@" + Column.Id;
            public const string Key = "@" + Column.Key;
            public const string Value = "@" + Column.Value;
            public const string ReminderDate = "@" + Column.ReminderDate;
            public const string IsReminderOn = "@" + Column.IsReminderOn;
            public const string CategoryId = "@" + Column.CategoryId;
            public const string Content = "@" + Column.Content;
            public const string ListOrder = "@" + Column.ListOrder;
            public const string Pinned = "@" + Column.Pinned;
            public const string IsDone = "@" + Column.IsDone;
            public const string CreationDate = "@" + Column.CreationDate;
            public const string ModificationDate = "@" + Column.ModificationDate;
            public const string Color = "@" + Column.Color;
            public const string Trashed = "@" + Column.Trashed;
        }

        #endregion Nested classes

        private static string AppDataFolder => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public const long DefaultListOrder = long.MaxValue / 2;
        public const long ListOrderInterval = 1_000_000_000_000;

        public const string OfflineDatabaseName = "TodoApp2Database.db";
        public const string OnlineDatabaseName = "TodoApp2Database-online.db";

        public static string OfflineDatabasePath { get; } = Path.Combine(AppDataFolder, OfflineDatabaseName);
        public static string OnlineDatabasePath { get; } = Path.Combine(AppDataFolder, OnlineDatabaseName);

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
                $"CREATE TABLE IF NOT EXISTS {Table.Settings} ( " +
                $" {Column.Id} INTEGER PRIMARY KEY, " +
                $" {Column.Key} TEXT, " +
                $" {Column.Value} TEXT " +
                $"); ";
            string createCategoryTable =
                $"CREATE TABLE IF NOT EXISTS {Column.Category} ( " +
                $" {Column.Id} INTEGER PRIMARY KEY, " +
                $" {Column.Name} TEXT, " +
                $" {Column.ListOrder} TEXT DEFAULT ('{DefaultListOrder}'), " +
                $" {Column.Trashed} INTEGER " +
                $"); ";
            string createTaskTable =
                $"CREATE TABLE IF NOT EXISTS {Table.Task} ( " +
                $" {Column.Id} INTEGER PRIMARY KEY, " +
                $" {Column.CategoryId} INTEGER, " +
                $" {Column.Content} TEXT, " +
                $" {Column.ListOrder} TEXT DEFAULT ('{DefaultListOrder}'), " +
                $" {Column.Pinned} INTEGER DEFAULT (0), " +
                $" {Column.IsDone} INTEGER DEFAULT (0), " +
                $" {Column.CreationDate} INTEGER, " +
                $" {Column.ModificationDate} INTEGER, " +
                $" {Column.Color} TEXT, " +
                $" {Column.Trashed} INTEGER DEFAULT (0), " +
                $" {Column.ReminderDate} INTEGER DEFAULT (0), " +
                $" {Column.IsReminderOn} INTEGER DEFAULT (0), " +
                $" FOREIGN KEY ({Column.CategoryId}) REFERENCES {Column.Category} ({Column.Id}) ON UPDATE CASCADE ON DELETE CASCADE " +
                $"); ";

            // Prepare database
            using (SQLiteCommand dbCommand = new SQLiteCommand(prepareCommand, m_Connection))
            using (SQLiteDataReader reader = dbCommand.ExecuteReader())
            {
            }

            // Create SETTINGS table
            using (SQLiteCommand dbCommand = new SQLiteCommand(createSettingsTable, m_Connection))
            using (SQLiteDataReader reader = dbCommand.ExecuteReader())
            {
            }

            // Create CATEGORY table
            using (SQLiteCommand dbCommand = new SQLiteCommand(createCategoryTable, m_Connection))
            using (SQLiteDataReader reader = dbCommand.ExecuteReader())
            {
            }

            // Create TASK table
            using (SQLiteCommand dbCommand = new SQLiteCommand(createTaskTable, m_Connection))
            using (SQLiteDataReader reader = dbCommand.ExecuteReader())
            {
            }

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

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT * FROM {Table.Settings}";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new SettingsModel
                        {
                            Id = reader.SafeGetInt(Column.Id),
                            Key = reader.SafeGetString(Column.Key),
                            Value = reader.SafeGetString(Column.Value)
                        });
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Gets the next available Id for a Settings record
        /// </summary>
        /// <returns></returns>
        public int GetSettingsNextId()
        {
            int nextId = int.MinValue;

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT * FROM {Table.Settings} ORDER BY {Column.Id} DESC LIMIT 1";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        nextId = reader.SafeGetInt(Column.Id) + 1;
                        break;
                    }
                }
            }

            return nextId;
        }

        /// <summary>
        /// Inserts a new record into the Settings table
        /// </summary>
        /// <param name="settings"></param>
        public void AddSetting(SettingsModel settings)
        {
            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"INSERT INTO {Table.Settings} ({Column.Id}, {Column.Key}, {Column.Value}) " +
                                      $" VALUES ({Parameter.Id}, {Parameter.Key}, {Parameter.Value});";

                command.Parameters.AddRange(new[]
                {
                    new SQLiteParameter(Parameter.Id, settings.Id),
                    new SQLiteParameter(Parameter.Key, settings.Key),
                    new SQLiteParameter(Parameter.Value, settings.Value)
                });

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                }
            }
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
                    using (SQLiteCommand command = new SQLiteCommand(m_Connection))
                    {
                        command.CommandText = $"INSERT INTO {Table.Settings} ({Column.Id}, {Column.Key}, {Column.Value}) " +
                                              $" VALUES ({Parameter.Id}, {Parameter.Key}, {Parameter.Value});";

                        command.Parameters.AddRange(new[]
                        {
                            new SQLiteParameter(Parameter.Id, settings.Id),
                            new SQLiteParameter(Parameter.Key, settings.Key),
                            new SQLiteParameter(Parameter.Value, settings.Value)
                        });

                        command.ExecuteNonQuery();
                    }
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
            bool result = false;

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"UPDATE {Table.Settings} SET " +
                                      $"  {Column.Key} = {Parameter.Key}, " +
                                      $"  {Column.Value} = {Parameter.Value} " +
                                      $" WHERE {Column.Id} = {Parameter.Id};";

                command.Parameters.AddRange(new[]
                {
                    new SQLiteParameter(Parameter.Id, settings.Id),
                    new SQLiteParameter(Parameter.Key, settings.Key),
                    new SQLiteParameter(Parameter.Value, settings.Value)
                });

                result = command.ExecuteNonQuery() > 0;
            }

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
                    using (SQLiteCommand command = new SQLiteCommand(m_Connection))
                    {
                        command.CommandText = $"UPDATE {Table.Settings} SET " +
                                              $"  {Column.Value} = {Parameter.Value} " +
                                              $" WHERE {Column.Key} = {Parameter.Key};";

                        command.Parameters.AddRange(new[]
                        {
                            new SQLiteParameter(Parameter.Key, setting.Key),
                            new SQLiteParameter(Parameter.Value, setting.Value)
                        });

                        modifiedItems += command.ExecuteNonQuery();
                    }
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
            bool result = false;
            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"DELETE FROM {Table.Settings} " +
                                      $" WHERE {Column.Id} = {Parameter.Id};";

                command.Parameters.Add(new SQLiteParameter(Parameter.Id, settings.Id));

                result = command.ExecuteNonQuery() > 0;
            }

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

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT * FROM {Column.Category} ORDER BY {Column.ListOrder}";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new CategoryListItemViewModel
                        {
                            Id = reader.SafeGetInt(Column.Id),
                            Name = reader.SafeGetString(Column.Name),
                            ListOrder = reader.SafeGetLongFromString(Column.ListOrder),
                            Trashed = reader.SafeGetBoolFromInt(Column.Trashed)
                        });
                    }
                }
            }

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

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT * FROM {Column.Category} WHERE {Column.Name} = {Parameter.Name}";
                command.Parameters.Add(new SQLiteParameter(Parameter.Name, name));

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = new CategoryListItemViewModel
                        {
                            Id = reader.SafeGetInt(Column.Id),
                            Name = reader.SafeGetString(Column.Name),
                            ListOrder = reader.SafeGetLongFromString(Column.ListOrder),
                            Trashed = reader.SafeGetBoolFromInt(Column.Trashed)
                        };
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Gets the category record with the provided ID from the Category table
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public CategoryListItemViewModel GetCategory(int categoryId)
        {
            CategoryListItemViewModel item = null;

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT * FROM {Column.Category} WHERE {Column.Id} = {Parameter.Id}";
                command.Parameters.Add(new SQLiteParameter(Parameter.Id, categoryId));

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = new CategoryListItemViewModel
                        {
                            Id = reader.SafeGetInt(Column.Id),
                            Name = reader.SafeGetString(Column.Name),
                            ListOrder = reader.SafeGetLongFromString(Column.ListOrder),
                            Trashed = reader.SafeGetBoolFromInt(Column.Trashed)
                        };
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Gets the next available Id for a Category record
        /// </summary>
        /// <returns></returns>
        public int GetCategoryNextId()
        {
            int nextId = int.MinValue;

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT * FROM {Column.Category} ORDER BY {Column.Id} DESC LIMIT 1";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        nextId = reader.SafeGetInt(Column.Id) + 1;
                        break;
                    }
                }
            }

            return nextId;
        }

        /// <summary>
        /// Gets the first ListOrder for a Category record
        /// </summary>
        /// <returns></returns>
        public long GetCategoryFirstListOrder()
        {
            return GetListOrder(Column.Category, true);
        }

        /// <summary>
        /// Gets the last ListOrder for a Category record
        /// </summary>
        /// <returns></returns>
        public long GetCategoryLastListOrder()
        {
            return GetListOrder(Column.Category, false);
        }

        /// <summary>
        /// Inserts a new record into the Category table
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public void AddCategory(CategoryListItemViewModel category)
        {
            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"INSERT INTO {Column.Category} ({Column.Id}, {Column.Name}, {Column.ListOrder}, {Column.Trashed}) " +
                                      $" VALUES ({Parameter.Id}, {Parameter.Name}, {Parameter.ListOrder}, {Parameter.Trashed});";

                command.Parameters.AddRange(new[]
                {
                    new SQLiteParameter(Parameter.Id, category.Id),
                    new SQLiteParameter(Parameter.Name, category.Name),
                    new SQLiteParameter(Parameter.ListOrder, category.ListOrder.ToString("D19")),
                    new SQLiteParameter(Parameter.Trashed, category.Trashed)
                });

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                }
            }
        }

        /// <summary>
        /// Updates a record in the Category table
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public bool UpdateCategory(CategoryListItemViewModel category)
        {
            bool result = false;
            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"UPDATE {Column.Category} SET " +
                                      $" {Column.Name} = {Parameter.Name}, " +
                                      $" {Column.ListOrder} = {Parameter.ListOrder}, " +
                                      $" {Column.Trashed} = {Parameter.Trashed} " +
                                      $" WHERE {Column.Id} IS {Parameter.Id};";

                command.Parameters.AddRange(new[]
                {
                    new SQLiteParameter(Parameter.Name, category.Name),
                    new SQLiteParameter(Parameter.ListOrder, category.ListOrder.ToString("D19")),
                    new SQLiteParameter(Parameter.Trashed, category.Trashed),
                    new SQLiteParameter(Parameter.Id, category.Id)
                });

                result = command.ExecuteNonQuery() > 0;
            }

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
                    using (SQLiteCommand command = new SQLiteCommand(m_Connection))
                    {
                        command.CommandText = $"UPDATE {Column.Category} SET " +
                                              $" {Column.ListOrder} = {Parameter.ListOrder} " +
                                              $" WHERE {Column.Id} = {Parameter.Id};";

                        command.Parameters.AddRange(new[]
                        {
                            new SQLiteParameter(Parameter.Id, category.Id),
                            new SQLiteParameter(Parameter.ListOrder, category.ListOrder.ToString("D19"))
                        });

                        modifiedItems += command.ExecuteNonQuery();
                    }
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
        public List<TaskListItemViewModel> GetActiveTasks()
        {
            List<TaskListItemViewModel> items = new List<TaskListItemViewModel>();

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT * FROM {Table.Task} " +
                                      $" WHERE {Column.Trashed} = 0 " +
                                      $" ORDER BY {Column.ListOrder} ;";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TaskListItemViewModel readTask = ReadTask(reader);
                        items.Add(readTask);
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Gets every task from the given category
        /// </summary>
        /// <returns></returns>
        public List<TaskListItemViewModel> GetTasksFromCategory(CategoryListItemViewModel category)
        {
            List<TaskListItemViewModel> items = GetTasksFromCategory(category.Id);

            return items;
        }

        /// <summary>
        /// Gets every task from the given category
        /// </summary>
        /// <returns></returns>
        public List<TaskListItemViewModel> GetTasksFromCategory(string categoryName)
        {
            CategoryListItemViewModel category = GetCategory(categoryName);
            List<TaskListItemViewModel> items = GetTasksFromCategory(category);

            return items;
        }

        /// <summary>
        /// Gets every task from the given category
        /// </summary>
        /// <returns></returns>
        public List<TaskListItemViewModel> GetTasksFromCategory(int categoryId)
        {
            List<TaskListItemViewModel> items = new List<TaskListItemViewModel>();

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT * FROM {Table.Task} " +
                                      $" WHERE {Column.CategoryId} = {categoryId} " +
                                      $" ORDER BY {Column.ListOrder} ;";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TaskListItemViewModel readTask = ReadTask(reader);
                        items.Add(readTask);
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Gets every task from the given category
        /// </summary>
        /// <returns></returns>
        public List<TaskListItemViewModel> GetActiveTasksFromCategory(int categoryId)
        {
            List<TaskListItemViewModel> items = new List<TaskListItemViewModel>();

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT * FROM {Table.Task} " +
                                      $" WHERE {Column.CategoryId} = {categoryId} " +
                                      $" AND {Column.Trashed} = 0 " +
                                      $" ORDER BY {Column.ListOrder} ;";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TaskListItemViewModel readTask = ReadTask(reader);
                        items.Add(readTask);
                    }
                }
            }

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

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT * FROM {Table.Task} WHERE {Column.Id} = {Parameter.Id} ;";
                command.Parameters.Add(new SQLiteParameter(Parameter.Id, id));

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        readTask = ReadTask(reader);
                    }
                }
            }

            return readTask;
        }

        /// <summary>
        /// Gets the next available Id for a Task record
        /// </summary>
        /// <returns></returns>
        public int GetTaskNextId()
        {
            int nextId = 0;

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT * FROM {Table.Task} ORDER BY {Column.Id} DESC LIMIT 1";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        nextId = reader.SafeGetInt(Column.Id) + 1;
                        break;
                    }
                }
            }

            return nextId;
        }

        /// <summary>
        /// Gets the first ListOrder for a Task record
        /// </summary>
        /// <returns></returns>
        public long GetTaskFirstListOrder()
        {
            return GetListOrder(Table.Task, true);
        }

        /// <summary>
        /// Gets the last ListOrder for a Task record
        /// </summary>
        /// <returns></returns>
        public long GetTaskLastListOrder()
        {
            return GetListOrder(Table.Task, false);
        }

        /// <summary>
        /// Creates a new Task and persists it into the Task table
        /// </summary>
        /// <param name="taskContent">The content of the task</param>
        /// <param name="categoryId">The category of the task</param>
        /// <returns>The created task.</returns>
        public TaskListItemViewModel CreateTask(string taskContent, int categoryId)
        {
            TaskListItemViewModel task = new TaskListItemViewModel
            {
                Id = GetTaskNextId(),
                CategoryId = categoryId,
                Content = taskContent,
                CreationDate = DateTime.Now.Ticks,
                ModificationDate = DateTime.Now.Ticks,
                Color = "Transparent",
                // The task is inserted at the top of the list by default
                ListOrder = GetTaskFirstListOrder() - ListOrderInterval
            };

            AddTask(task);

            return task;
        }

        /// <summary>
        /// Updates a record in the Task table
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool UpdateTask(TaskListItemViewModel task)
        {
            bool result = false;
            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"UPDATE {Table.Task} SET " +
                                      $"  {Column.CategoryId} = {Parameter.CategoryId}, " +
                                      $"  {Column.Content} = {Parameter.Content}, " +
                                      $"  {Column.ListOrder} = {Parameter.ListOrder}, " +
                                      $"  {Column.Pinned} = {Parameter.Pinned}, " +
                                      $"  {Column.IsDone} = {Parameter.IsDone}, " +
                                      $"  {Column.CreationDate} = {Parameter.CreationDate}, " +
                                      $"  {Column.ModificationDate} = {Parameter.ModificationDate}, " +
                                      $"  {Column.Color} = {Parameter.Color}, " +
                                      $"  {Column.Trashed} = {Parameter.Trashed}, " +
                                      $"  {Column.ReminderDate} = {Parameter.ReminderDate}, " +
                                      $"  {Column.IsReminderOn} = {Parameter.IsReminderOn} " +
                                      $" WHERE {Column.Id} = {Parameter.Id};";
                command.Parameters.AddRange(CreateTaskParameterList(task));

                result = command.ExecuteNonQuery() > 0;
            }

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
                    using (SQLiteCommand command = new SQLiteCommand(m_Connection))
                    {
                        command.CommandText = $"UPDATE {Table.Task} SET " +
                                              $"  {Column.CategoryId} = {Parameter.CategoryId}, " +
                                              $"  {Column.Content} = {Parameter.Content}, " +
                                              $"  {Column.ListOrder} = {Parameter.ListOrder}, " +
                                              $"  {Column.Pinned} = {Parameter.Pinned}, " +
                                              $"  {Column.IsDone} = {Parameter.IsDone}, " +
                                              $"  {Column.CreationDate} = {Parameter.CreationDate}, " +
                                              $"  {Column.ModificationDate} = {Parameter.ModificationDate}, " +
                                              $"  {Column.Color} = {Parameter.Color}, " +
                                              $"  {Column.Trashed} = {Parameter.Trashed}, " +
                                              $"  {Column.ReminderDate} = {Parameter.ReminderDate}, " +
                                              $"  {Column.IsReminderOn} = {Parameter.IsReminderOn} " +
                                              $" WHERE {Column.Id} = {Parameter.Id};";

                        command.Parameters.AddRange(CreateTaskParameterList(task));

                        modifiedItems += command.ExecuteNonQuery();
                    }
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
                    using (SQLiteCommand command = new SQLiteCommand(m_Connection))
                    {
                        command.CommandText = $"UPDATE {Table.Task} SET " +
                                              $" {Column.ListOrder} = {Parameter.ListOrder} " +
                                              $" WHERE {Column.Id} = {Parameter.Id};";
                        command.Parameters.AddRange(new[]
                        {
                            new SQLiteParameter(Parameter.Id, todoTask.Id),
                            new SQLiteParameter(Parameter.ListOrder, todoTask.ListOrder.ToString("D19"))
                        });

                        modifiedItems += command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }

            return modifiedItems;
        }

        /// <summary>
        /// Inserts a new record into the Task table
        /// </summary>
        /// <param name="taskListItem"></param>
        /// <returns></returns>
        private void AddTask(TaskListItemViewModel taskListItem)
        {
            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"INSERT INTO {Table.Task} " +
                                      $" ({Column.Id}, {Column.CategoryId}, {Column.Content}, " +
                                      $" {Column.ListOrder}, {Column.Pinned}, {Column.IsDone}, " +
                                      $" {Column.CreationDate}, {Column.ModificationDate}, {Column.Color}, " +
                                      $" {Column.Trashed}, {Column.ReminderDate}, {Column.IsReminderOn}) " +
                                      $" VALUES ({Parameter.Id}, {Parameter.CategoryId}, {Parameter.Content}, " +
                                      $" {Parameter.ListOrder}, {Parameter.Pinned}, {Parameter.IsDone}, " +
                                      $" {Parameter.CreationDate}, {Parameter.ModificationDate}, {Parameter.Color}, " +
                                      $" {Parameter.Trashed}, {Parameter.ReminderDate}, {Parameter.IsReminderOn});";

                command.Parameters.AddRange(CreateTaskParameterList(taskListItem));

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                }
            }
        }

        /// <summary>
        /// Reads a task from the database, using an <see cref="SQLiteDataReader"/>.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private TaskListItemViewModel ReadTask(SQLiteDataReader reader)
        {
            TaskListItemViewModel readTask = new TaskListItemViewModel
            {
                Id = reader.SafeGetInt(Column.Id),
                CategoryId = reader.SafeGetInt(Column.CategoryId),
                Content = reader.SafeGetString(Column.Content),
                ListOrder = reader.SafeGetLongFromString(Column.ListOrder),
                Pinned = reader.SafeGetBoolFromInt(Column.Pinned),
                IsDone = reader.SafeGetBoolFromInt(Column.IsDone),
                CreationDate = reader.SafeGetLong(Column.CreationDate),
                ModificationDate = reader.SafeGetLong(Column.ModificationDate),
                Color = reader.SafeGetString(Column.Color),
                Trashed = reader.SafeGetBoolFromInt(Column.Trashed),
                ReminderDate = reader.SafeGetLong(Column.ReminderDate),
                IsReminderOn = reader.SafeGetBoolFromInt(Column.IsReminderOn)
            };

            return readTask;
        }

        /// <summary>
        /// Creates an SQLite parameter list for a task.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private SQLiteParameter[] CreateTaskParameterList(TaskListItemViewModel task)
        {
            return new[]
            {
                new SQLiteParameter(Parameter.Id, task.Id),
                new SQLiteParameter(Parameter.CategoryId, task.CategoryId),
                new SQLiteParameter(Parameter.Content, task.Content),
                new SQLiteParameter(Parameter.ListOrder, task.ListOrder.ToString("D19")),
                new SQLiteParameter(Parameter.Pinned, task.Pinned),
                new SQLiteParameter(Parameter.IsDone, task.IsDone),
                new SQLiteParameter(Parameter.CreationDate, task.CreationDate),
                new SQLiteParameter(Parameter.ModificationDate, task.ModificationDate),
                new SQLiteParameter(Parameter.Color, task.Color),
                new SQLiteParameter(Parameter.Trashed, task.Trashed),
                new SQLiteParameter(Parameter.ReminderDate, task.ReminderDate),
                new SQLiteParameter(Parameter.IsReminderOn, task.IsReminderOn)
            };
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
            long listOrder = DefaultListOrder;

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT * FROM {table} WHERE {Column.Trashed} = 0 ORDER BY {Column.ListOrder} {ordering} LIMIT 1";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listOrder = reader.SafeGetLongFromString(Column.ListOrder);
                        break;
                    }
                }
            }

            return listOrder;
        }

        public void Dispose()
        {
            m_Connection?.Close();
            m_Connection?.Dispose();
        }
    }
}