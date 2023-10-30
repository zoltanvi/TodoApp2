using System;
using TodoApp2.Common;
using TodoApp2.Entity.Model;
using TodoApp2.Entity.Query;

namespace TodoApp2.Entity
{
    public class DbSetMapping<TModel>
        where TModel : class, new()
    {
        protected internal DbSetModelBuilder<TModel> ModelBuilder { get; }
        
        internal string TableName { get; }

        public DbSetMapping(string tableName)
        {
            ThrowHelper.ThrowIfNull(tableName);
            TableName = tableName;
            ModelBuilder = new DbSetModelBuilder<TModel>(TableName);
        }

        internal void CheckEmptyMapping()
        {
            if (ModelBuilder.Properties.Count == 0)
            {
                throw new InvalidOperationException("Cannot create empty mapping!");
            }
        }

        internal void CreateIfNotExists(DbConnection connection)
        {
            ThrowHelper.ThrowIfNull(connection);
            string query = QueryBuilder.CreateIfNotExists(ModelBuilder);
            QueryExecutor.ExecuteQuery(connection, query);
        }
    }
}
