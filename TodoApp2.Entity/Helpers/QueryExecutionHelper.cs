using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TodoApp2.Entity.Extensions;

namespace TodoApp2.Entity.Helpers
{
    internal static class QueryExecutionHelper
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
                    throw;
                }
            }
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
