using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using TodoApp2.Entity.Extensions;

namespace TodoApp2.Entity.Query
{
    internal static class QueryExecutor
    {
        public static void ExecuteQuery(DbConnection connection, string query)
        {
            using (SQLiteCommand dbCommand = new SQLiteCommand(query, connection.InternalConnection))
            {
                try
                {
                    dbCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                    throw;
                }
            }
        }

        internal static int ExecuteQuery(DbConnection connection, string query, SQLiteParameter[] parameters)
        {
            int successful = 0;

            using (SQLiteCommand command = new SQLiteCommand(connection.InternalConnection))
            {
                try
                {
                    command.CommandText = query;
                    command.Parameters.AddRange(parameters);
                    successful = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                    throw;
                }
            }

            return successful;
        }

        public static TModel GetItemWithQuery<TModel>(DbConnection connection, string query)
            where TModel : class, new()
        {
            TModel item = null;

            using (SQLiteCommand command = new SQLiteCommand(connection.InternalConnection))
            {
                command.CommandText = query;

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = reader.ReadProperties<TModel>();
                        break;
                    }
                }
            }

            return item;
        }

        public static List<TModel> GetItemsWithQuery<TModel>(DbConnection connection, string query)
            where TModel : class, new()
        {
            List<TModel> items = new List<TModel>();

            using (SQLiteCommand command = new SQLiteCommand(connection.InternalConnection))
            {
                command.CommandText = query;

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TModel readItem = reader.ReadProperties<TModel>();
                        items.Add(readItem);
                    }
                }
            }

            return items;
        }
    }
}
