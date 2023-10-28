using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using TodoApp2.Entity.Extensions;

namespace TodoApp2.Entity
{
    public class DbSet<TModel> where TModel : class, new()
    {
        private DbConnection _connection;

        internal string TableName;
        internal DbSetConfiguration<TModel> DbSetConfiguration;

        /// <summary>
        /// Creates an abstract representation of an actual database table.
        /// </summary>
        /// <param name="connection">The db connection.</param>
        /// <param name="tableName">The name of the table to represent. It MUST match the table name in the db!</param>
        public DbSet(DbConnection connection, string tableName)
        {
            _connection = connection;
            TableName = tableName;
        }

        public List<TModel> GetAll()
        {
            string query = $"SELECT * FROM {TableName} ";

            return GetItemsWithQuery(query);
        }


        public DbSetConfiguration<TModel> Configure()
        {
            if (DbSetConfiguration == null)
            {
                DbSetConfiguration = new DbSetConfiguration<TModel>(this);
            }

            return DbSetConfiguration;
        }

        // TODO: Separate creation from DbSet -> ctor param DbSetMapping ?
        public void CreateIfNotExists()
        {
            string query = DbSetQueryBuilder.BuildCreateIfNotExists(this);
            ExecuteQuery(query);
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
