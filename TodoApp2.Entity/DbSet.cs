using System.Collections.Generic;
using System.Data.SQLite;
using TodoApp2.Entity.Extensions;

namespace TodoApp2.Entity
{
    public class DbSet<TModel> where TModel : class, new()
    {
        private DbConnection _connection;
        private string _tableName;

        /// <summary>
        /// Creates an abstract representation of an actual database table.
        /// </summary>
        /// <param name="connection">The db connection.</param>
        /// <param name="tableName">The name of the table to represent. It MUST match the table name in the db!</param>
        public DbSet(DbConnection connection, string tableName)
        {
            _connection = connection;
            _tableName = tableName;
        }

        public List<TModel> GetAll()
        {
            string query = $"SELECT * FROM {_tableName} ";

            return GetItemsWithQuery(query);
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
