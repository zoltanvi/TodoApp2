using System.Collections.Generic;
using TodoApp2.Common;
using TodoApp2.Entity.Info;
using TodoApp2.Entity.Query;

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

            return QueryExecutor.GetItemsWithQuery<TModel>(_connection, query);
        }

        public int Count()
        {
            string query = $"SELECT COUNT(*) AS Value FROM {TableName} ";
            var valueModel = QueryExecutor.GetItemWithQuery<SingleIntModel>(_connection, query);
            return valueModel.Value;
        }

        public bool Add(TModel model)
        {
            var primaryKeyName = DbSetMapping.ModelBuilder.GetPrimaryKeyName();
            bool success = QueryExecutor.InsertItem<TModel>(_connection, model, TableName, primaryKeyName);
            return success;
        }

    }
}
