using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    public abstract partial class BaseDataAccess
    {
        protected readonly SQLiteConnection _connection;

        public const long DefaultListOrder = long.MaxValue / 2;
        public const long ListOrderInterval = 1_000_000_000_000;

        protected BaseDataAccess(SQLiteConnection connection)
        {
            _connection = connection;
        }

        protected void ExecuteCommand(string command)
        {
            using (SQLiteCommand dbCommand = new SQLiteCommand(command, _connection))
            using (SQLiteDataReader reader = dbCommand.ExecuteReader())
            {
            }
        }

        /// <summary>
        /// Gets the first or last ListOrder for a <see cref="table"/> record.
        /// </summary>
        /// <param name="table">The database table to query.</param>
        /// <param name="first">
        /// If true, queries the first ListOrder, 
        /// otherwise queries the last ListOrder.
        /// </param>
        /// <returns>Returns the query result.</returns>
        protected long GetListOrder(string table, bool first)
        {
            string ordering = first ? string.Empty : "DESC";
            long listOrder = DefaultListOrder;

            using (SQLiteCommand command = new SQLiteCommand(_connection))
            {
                command.CommandText =
                    $"SELECT * FROM {table} " +
                    $"WHERE {Column.Trashed} = 0 " +
                    $"ORDER BY {Column.ListOrder} {ordering} LIMIT 1";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        listOrder = reader.SafeGetLongFromString(Column.ListOrder);
                    }
                }
            }

            return listOrder;
        }

        protected List<T> GetItemsWithQuery<T>(string query, Func<SQLiteDataReader, T> itemReader)
        {
            List<T> items = new List<T>();

            using (SQLiteCommand command = new SQLiteCommand(_connection))
            {
                command.CommandText = query;

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T readItem = itemReader(reader);
                        items.Add(readItem);
                    }
                }
            }

            return items;
        }
    }

    // This is only to visually separate the nested structs from actual logic
    public abstract partial class BaseDataAccess
    {
        protected struct Table
        {
            public const string Task = "Task";
            public const string Settings = "Setting";
            public const string Category = "Category";
            public const string NoteCategory = "NoteCategory";
            public const string Note = "Note";
        }

        protected struct Column
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
            public const string BorderColor = "BorderColor";
            public const string BackgroundColor = "BackgroundColor";
            public const string Trashed = "Trashed";
            public const string CategoryId = "CategoryId";
            public const string Title = "Title";
            public const string DatabaseVersion = "user_version";
        }

        protected struct Parameter
        {
            public const string Name = "@" + Column.Name;
            public const string Id = "@" + Column.Id;
            public const string Key = "@" + Column.Key;
            public const string NewKey = "@New" + Column.Key;
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
            public const string BorderColor = "@" + Column.BorderColor;
            public const string BackgroundColor = "@" + Column.BackgroundColor;
            public const string Trashed = "@" + Column.Trashed;
            public const string Title = "@" + Column.Title;
        }

    }
}
