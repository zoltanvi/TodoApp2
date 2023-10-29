using System;
using System.Data.SQLite;
using TodoApp2.Common;

namespace TodoApp2.Entity
{
    public class DbSetMapping<TModel>
        where TModel : class, new()
    {
        private DbSetModelBuilder<TModel> _modelBuilder;
        
        internal string TableName;

        protected internal DbSetModelBuilder<TModel> ModelBuilder => _modelBuilder ?? (_modelBuilder = new DbSetModelBuilder<TModel>());

        public DbSetMapping(string tableName)
        {
            ThrowHelper.ThrowIfNull(tableName);
            TableName = tableName;
        }

        internal void CheckEmptyMapping()
        {
            if (_modelBuilder == null || _modelBuilder.Properties.Count == 0)
            {
                throw new InvalidOperationException("Cannot create empty mapping!");
            }
        }

        internal void CreateIfNotExists(DbConnection connection)
        {
            ThrowHelper.ThrowIfNull(connection);
            string query = DbSetQueryBuilder.BuildCreateIfNotExists(this);
            ExecuteQuery(connection, query);
        }

        private void ExecuteQuery(DbConnection connection, string query)
        {
            using (SQLiteCommand dbCommand = new SQLiteCommand(query, connection.InternalConnection))
            using (SQLiteDataReader reader = dbCommand.ExecuteReader())
            {
            }
        }
    }
}
