using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace TodoApp2.Core
{
    // CRUD = Create, Read, Update, Delete
    public class TaskDataAccess : BaseDataAccess
    {
        public TaskDataAccess(SQLiteConnection connection) : base(connection)
        {
        }

        public bool IsTaskTableExists()
        {
            bool exists = false;
            string command = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{Table.Task}' ";

            // Get user version
            using (SQLiteCommand dbCommand = new SQLiteCommand(command, _connection))
            using (SQLiteDataReader reader = dbCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    // If the query returns the table's name, it exists.
                    exists = reader.SafeGetString(Column.Name) != string.Empty;
                    break;
                }
            }

            return exists;
        }

        // Create

        public void CreateTaskTable()
        {
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
                $" {Column.Color} TEXT DEFAULT \"Transparent\", " +
                $" {Column.BorderColor} TEXT DEFAULT \"Transparent\", " +
                $" {Column.BackgroundColor} TEXT DEFAULT \"Transparent\", " +
                $" {Column.Trashed} INTEGER DEFAULT (0), " +
                $" {Column.ReminderDate} INTEGER DEFAULT (0), " +
                $" {Column.IsReminderOn} INTEGER DEFAULT (0), " +
                $" FOREIGN KEY ({Column.CategoryId}) REFERENCES {Column.Category} ({Column.Id}) ON UPDATE CASCADE ON DELETE CASCADE " +
                $"); ";

            ExecuteCommand(createTaskTable);
        }

        public TaskViewModel CreateTask(string taskContent, int categoryId)
        {
            TaskViewModel task = new TaskViewModel
            {
                Id = GetNextId(),
                CategoryId = categoryId,
                Content = taskContent,
                CreationDate = DateTime.Now.Ticks,
                ModificationDate = DateTime.Now.Ticks,
                Color = GlobalConstants.ColorName.Transparent,
                BorderColor = GlobalConstants.ColorName.Transparent,
                BackgroundColor = GlobalConstants.ColorName.Transparent,
                // The task is inserted at the top of the list by default
                ListOrder = GetListOrder(Table.Task, true) - ListOrderInterval
            };

            AddTask(task);

            return task;
        }

        // Read

        public List<TaskViewModel> GetActiveTasks()
        {
            string query =
                $"SELECT * FROM {Table.Task} " +
                $" WHERE {Column.Trashed} = 0 " +
                $" ORDER BY {Column.ListOrder} ;";
            
            return GetItemsWithQuery(query, ReadTask);
        }

        public List<TaskViewModel> GetActiveTasks(int categoryId)
        {
            string query =
                $"SELECT * FROM {Table.Task} " +
                $" WHERE {Column.CategoryId} = {categoryId} " +
                $" AND {Column.Trashed} = 0 " +
                $" ORDER BY {Column.ListOrder} ;";

            return GetItemsWithQuery(query, ReadTask);
        }

        public List<TaskViewModel> GetActiveTasksWithReminder()
        {
            string query =
                $"SELECT * FROM {Table.Task} " +
                $" WHERE {Column.Trashed} = 0 " +
                $" AND {Column.IsReminderOn} = 1 " +
                $" ORDER BY {Column.ListOrder} ;";

            return GetItemsWithQuery(query, ReadTask);
        }

        public TaskViewModel GetTask(int id)
        {
            TaskViewModel readTask = null;

            using (SQLiteCommand command = new SQLiteCommand(_connection))
            {
                command.CommandText =
                    $"SELECT * FROM {Table.Task} " +
                    $"WHERE {Column.Id} = {Parameter.Id} ;";

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

        // Update

        public bool UpdateTask(TaskViewModel task)
        {
            bool result = false;
            using (SQLiteCommand command = new SQLiteCommand(_connection))
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
                                      $"  {Column.BorderColor} = {Parameter.BorderColor}, " +
                                      $"  {Column.BackgroundColor} = {Parameter.BackgroundColor}, " +
                                      $"  {Column.Trashed} = {Parameter.Trashed}, " +
                                      $"  {Column.ReminderDate} = {Parameter.ReminderDate}, " +
                                      $"  {Column.IsReminderOn} = {Parameter.IsReminderOn} " +
                                      $" WHERE {Column.Id} = {Parameter.Id};";
                command.Parameters.AddRange(CreateTaskParameterList(task));

                result = command.ExecuteNonQuery() > 0;
            }

            return result;
        }

        public int UpdateTaskList(IEnumerable<TaskViewModel> taskList)
        {
            int modifiedItems = 0;

            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = _connection.BeginTransaction())
            {
                foreach (TaskViewModel task in taskList)
                {
                    modifiedItems += UpdateTask(task) ? 1 : 0;
                }

                transaction.Commit();
            }

            return modifiedItems;
        }

        public int UpdateTaskListOrders(IEnumerable<TaskViewModel> taskList)
        {
            int modifiedItems = 0;

            // Using transaction to write the database only once
            using (SQLiteTransaction transaction = _connection.BeginTransaction())
            {
                foreach (var todoTask in taskList)
                {
                    using (SQLiteCommand command = new SQLiteCommand(_connection))
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

        private void AddTask(TaskViewModel taskListItem)
        {
            using (SQLiteCommand command = new SQLiteCommand(_connection))
            {
                command.CommandText = $"INSERT INTO {Table.Task} " +
                                      $" ({Column.Id}, {Column.CategoryId}, {Column.Content}, " +
                                      $" {Column.ListOrder}, {Column.Pinned}, {Column.IsDone}, " +
                                      $" {Column.CreationDate}, {Column.ModificationDate}, {Column.Color}, " +
                                      $" {Column.BorderColor}, {Column.BackgroundColor}, " +
                                      $" {Column.Trashed}, {Column.ReminderDate}, {Column.IsReminderOn}) " +
                                      $" VALUES ({Parameter.Id}, {Parameter.CategoryId}, {Parameter.Content}, " +
                                      $" {Parameter.ListOrder}, {Parameter.Pinned}, {Parameter.IsDone}, " +
                                      $" {Parameter.CreationDate}, {Parameter.ModificationDate}, {Parameter.Color}, " +
                                      $" {Parameter.BorderColor}, {Parameter.BackgroundColor}, " +
                                      $" {Parameter.Trashed}, {Parameter.ReminderDate}, {Parameter.IsReminderOn});";

                command.Parameters.AddRange(CreateTaskParameterList(taskListItem));

                command.ExecuteNonQuery();
            }
        }

        private int GetNextId()
        {
            int nextId = 0;

            using (SQLiteCommand command = new SQLiteCommand(_connection))
            {
                command.CommandText =
                    $"SELECT * FROM {Table.Task} " +
                    $"ORDER BY {Column.Id} DESC LIMIT 1";

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

        private TaskViewModel ReadTask(SQLiteDataReader reader)
        {
            TaskViewModel readTask = new TaskViewModel
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
                BorderColor = reader.SafeGetString(Column.BorderColor),
                BackgroundColor = reader.SafeGetString(Column.BackgroundColor),
                Trashed = reader.SafeGetBoolFromInt(Column.Trashed),
                ReminderDate = reader.SafeGetLong(Column.ReminderDate),
                IsReminderOn = reader.SafeGetBoolFromInt(Column.IsReminderOn)
            };

            return readTask;
        }

        private SQLiteParameter[] CreateTaskParameterList(TaskViewModel task)
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
                new SQLiteParameter(Parameter.BorderColor, task.BorderColor),
                new SQLiteParameter(Parameter.BackgroundColor, task.BackgroundColor),
                new SQLiteParameter(Parameter.Trashed, task.Trashed),
                new SQLiteParameter(Parameter.ReminderDate, task.ReminderDate),
                new SQLiteParameter(Parameter.IsReminderOn, task.IsReminderOn)
            };
        }
    }
}
