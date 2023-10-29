using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using TodoApp2.Common;
using TodoApp2.Entity.Extensions;

namespace TodoApp2.Entity
{
    public class DbSet<TModel> 
        where TModel : class, new()
    {
        private DbConnection _connection;

        internal string TableName;
        internal DbSetMapping<TModel> DbSetMapping;

        /// <summary>
        /// Creates an abstract representation of an actual database table.
        /// </summary>
        /// <param name="connection">The db connection.</param>
        /// <param name="dbSetMapping">The db set mapping.</param>
        public DbSet(DbConnection connection, DbSetMapping<TModel> dbSetMapping)
        {
            ThrowHelper.ThrowIfNull(connection);
            ThrowHelper.ThrowIfNull(dbSetMapping);

            _connection = connection;
            DbSetMapping = dbSetMapping;
            TableName = dbSetMapping.TableName;
            DbSetMapping.CheckEmptyMapping();
            DbSetMapping.CreateIfNotExists(connection);
        }

        public List<TModel> GetAll()
        {
            string query = $"SELECT * FROM {TableName} ";

            return GetItemsWithQuery(query);
        }

        private void ExecuteQuery(string query)
        {
            using (SQLiteCommand dbCommand = new SQLiteCommand(query, _connection.InternalConnection))
            using (SQLiteDataReader reader = dbCommand.ExecuteReader())
            {
            }
        }

        private List<TModel> GetItemsWithQuery(string query)
        {
            List<TModel> items = new List<TModel>();

            using (SQLiteCommand command = new SQLiteCommand(_connection.InternalConnection))
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
